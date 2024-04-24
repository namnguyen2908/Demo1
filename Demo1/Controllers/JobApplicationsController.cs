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
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobApplications
        public async Task<IActionResult> Index()
        {
            // Lấy ID của người dùng đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            // Tìm người dùng trong cơ sở dữ liệu
            var user = _context.Users.Find(userId);

            // Tạo biến để lưu danh sách các Job Applications phù hợp với vai trò của người dùng
            IQueryable<JobApplication> userApplications;

            // Kiểm tra nếu người dùng là JobSeeker
            if (user.UserRole == "JobSeeker")
            {
                // Lấy danh sách các Job Applications mà người dùng đã tạo
                userApplications = _context.JobApplication
                    .Where(j => j.ApplicationUserId == userId)
                    .Include(j => j.ApplicationUser)
                    .Include(j => j.JobListing);
            }
            else
            {
                // Nếu người dùng không phải là JobSeeker, chuyển họ đến trang chính
                return RedirectToAction("Index", "Home");
            }

            // Trả về danh sách Job Applications phù hợp với vai trò của người dùng
            return View(await userApplications.ToListAsync());
        }

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobApplication == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication
                .Include(j => j.ApplicationUser)
                .Include(j => j.JobListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // GET: JobApplications/Create
        public IActionResult Create(int jobListingId)
        {
            // Lấy ID của người dùng đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra vai trò của người dùng
            var user = _context.Users.Find(userId);

            // Chỉ cho phép người dùng đăng nhập với vai trò là JobSeeker tạo job application
            if (user.UserRole == "JobSeeker")
            {
                var jobApplication = new JobApplication
                {
                    ApplicationUserId = userId,
                    JobListingID = jobListingId // Tự động điền vào JobListingID từ query string
                };

                ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userId);
                ViewData["JobListingID"] = new SelectList(_context.JobListing, "Id", "Id", jobListingId); // Đảm bảo JobListingID được chọn trong dropdown list
                return View(jobApplication);
            }
            else
            {
                // Nếu người dùng không phải là JobSeeker, chuyển họ đến một trang thông báo hoặc chuyển hướng khác
                return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang chính của ứng dụng
            }
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Coverletter,JobListingID,ApplicationUserId")] JobApplication jobApplication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobApplication.ApplicationUserId);
            ViewData["JobListingID"] = new SelectList(_context.JobListing, "Id", "Id", jobApplication.JobListingID);
            return View(jobApplication);
        }

        // GET: JobApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobApplication == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            // Lấy ID của người dùng đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kiểm tra vai trò của người dùng
            var user = _context.Users.Find(userId);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobApplication.ApplicationUserId);
            ViewData["JobListingID"] = new SelectList(_context.JobListing, "Id", "Id", jobApplication.JobListingID);
            return View(jobApplication);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Coverletter,JobListingID,ApplicationUserId")] JobApplication jobApplication)
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(userId);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobApplicationExists(jobApplication.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jobApplication.ApplicationUserId);
            ViewData["JobListingID"] = new SelectList(_context.JobListing, "Id", "Id", jobApplication.JobListingID);
            return View(jobApplication);
        }

        // GET: JobApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobApplication == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication
                .Include(j => j.ApplicationUser)
                .Include(j => j.JobListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobApplication == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobApplication'  is null.");
            }
            var jobApplication = await _context.JobApplication.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplication.Remove(jobApplication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return (_context.JobApplication?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
