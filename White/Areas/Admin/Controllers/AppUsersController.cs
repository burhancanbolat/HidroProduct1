﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using White.Data;

namespace White.Areas.Admin.Controllers
{
    [Area("Admin")]

   
    public class AppUsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> userManager;
       

        public AppUsersController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
            
        }
        [Authorize(Roles = "Administrators")]
        // GET: Admin/AppUsers
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Users'  is null.");
        }




        // GET: Admin/AppUsers/Create
        [Authorize(Roles = "Administrators,Kaydediciler")]
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Admin/AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators,Kaydediciler")]
        public async Task<IActionResult> Create(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                appUser.Id = Guid.NewGuid();
                appUser.Email = appUser.UserName;
                appUser.EmailConfirmed = true;
                await userManager.CreateAsync(appUser,appUser.Password);
               await userManager.AddClaimAsync(appUser, new Claim("Name", appUser.Name));
                if (appUser.IsAdministrator)
                    await userManager.AddToRoleAsync(appUser, "Administrators");
                if (appUser.IsKaydedici)
                    await userManager.AddToRoleAsync(appUser, "Kaydediciler");
                if (appUser.IsSilici)
                    await userManager.AddToRoleAsync(appUser, "Siliciler");
                if (appUser.IsGüncelleyici)
                    await userManager.AddToRoleAsync(appUser, "Güncelleyiciler");
                if (appUser.IsGörüntüleyici)
                    await userManager.AddToRoleAsync(appUser, "Görüntüleyiciler");
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: Admin/AppUsers/Edit/5
        [Authorize(Roles = "Administrators,Güncelleyiciler")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        // POST: Admin/AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators,Güncelleyiciler")]
        public async Task<IActionResult> Edit(Guid id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
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
            return View(appUser);
        }

        // GET: Admin/AppUsers/Delete/5
        [Authorize(Roles = "Administrators,Siliciler")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: Admin/AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators,Siliciler")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'AppDbContext.Users'  is null.");
            }
            var appUser = await _context.Users.FindAsync(id);
            if (appUser != null)
            {
                _context.Users.Remove(appUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(Guid id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
