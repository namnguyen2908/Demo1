using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo1.Data;
using Demo1.Models;
using System.Security.Claims;

namespace Demo1.Controllers
{
    public class JobListingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobListingsController(ApplicationDbContext context)
        {
            _context = context;
        }






        public async Task<IActionResult> Apply(string searchString)
        {
            var joblistings = from j in _context.JobListing
                       select j;
            // Chỉ tìm kiếm theo tên của job listing
            if (!String.IsNullOrEmpty(searchString))
            {
                joblistings = joblistings.Where(j => j.Name.Contains(searchString));
            }
            var applicationDbContext = joblistings.Include(j => j.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }







        // GET: JobListings
        public async Task<IActionResult> Index(string searchString)
        {
            // Lấy ID của người dùng đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            // Tìm người dùng trong cơ sở dữ liệu
            var user = _context.Users.Find(userId);

            // Chỉ hiển thị những job listing của người dùng nếu họ là Employer
            if (user.UserRole == "Employer")
            {
                // Lọc job listing theo chuỗi tìm kiếm (nếu có) và người dùng đã tạo
                var joblistings = _context.JobListing.Where(j => j.ApplicationUserId == userId);

                // Chỉ tìm kiếm theo tên của job listing
                if (!String.IsNullOrEmpty(searchString))
                {
                    joblistings = joblistings.Where(j => j.Name.Contains(searchString));
                }

                // Include các đối tượng liên quan
                var applicationDbContext = joblistings.Include(j => j.ApplicationUser);

                // Trả về view với danh sách các job listing
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                // Nếu người dùng không phải là Employer, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }
        }








        // GET: JobListings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobListing == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing
                .Include(j => j.ApplicationUser)
                .Include(j => j.JobApplication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }








        // GET: JobListings/Create
        public IActionResult Create()
        {
            // Lấy ID của người dùng đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Tìm người dùng trong cơ sở dữ liệu
            var user = _context.Users.Find(userId);

            // Chỉ cho phép người dùng đăng nhập với vai trò là Employer tạo job listing
            if (user.UserRole == "Employer")
            {
                // Thiết lập ApplicationUserId cho job listing là ID của người dùng đăng nhập
                var jobListing = new JobListing
                {
                    ApplicationUserId = userId
                };

                ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userId);
                return View(jobListing);
            }
            else
            {
                // Nếu người dùng không phải là Employer, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }
        }

        // POST: JobListings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,RequiredQualifications,ApplicationDeadline,ApplicationUserId")] JobListing jobListing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobListing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobListing.ApplicationUserId);
            return View(jobListing);
        }








        // GET: JobListings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobListing == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing.FindAsync(id);
            if (jobListing == null)
            {
                return NotFound();
            }

            // Lấy ID của người dùng đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra vai trò của người dùng
            var user = _context.Users.Find(userId);
            if (user.UserRole != "Employer")
            {
                // Nếu người dùng không phải là Employer, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }

            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobListing.ApplicationUserId);
            return View(jobListing);
        }

        // POST: JobListings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,RequiredQualifications,ApplicationDeadline,ApplicationUserId")] JobListing jobListing)
        {
            if (id != jobListing.Id)
            {
                return NotFound();
            }

            // Kiểm tra vai trò của người dùng
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(userId);
            if (user.UserRole != "Employer")
            {
                // Nếu người dùng không phải là Employer, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobListing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobListingExists(jobListing.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobListing.ApplicationUserId);
            return View(jobListing);
        }










        // GET: JobListings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobListing == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing
                .Include(j => j.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            // Kiểm tra vai trò của người dùng
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(userId);
            if (user.UserRole != "Employer")
            {
                // Nếu người dùng không phải là Employer, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }

            return View(jobListing);
        }

        // POST: JobListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobListing == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobListing'  is null.");
            }

            // Kiểm tra vai trò của người dùng
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(userId);
            if (user.UserRole != "Employer")
            {
                // Nếu người dùng không phải là Employer, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }

            var jobListing = await _context.JobListing.FindAsync(id);
            if (jobListing != null)
            {
                _context.JobListing.Remove(jobListing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }






        private bool JobListingExists(int id)
        {
            return (_context.JobListing?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}