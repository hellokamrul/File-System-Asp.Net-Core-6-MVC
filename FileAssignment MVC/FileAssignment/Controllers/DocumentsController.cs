using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileAssignment.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FileAssignment.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace FileAssignment.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly MultipleFileSystemContext _context;
        private readonly IHostingEnvironment _env;

        public DocumentsController(MultipleFileSystemContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Documents
        public IActionResult Index()
        {
            var data = _context.Documents.ToList();
            return View(data);
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(FileCreateModel model)
        {
            if(ModelState.IsValid)
            {
                var path = _env.WebRootPath;
                var filePath = "Content/Files/" + model.FilePath.FileName;
                var fullPath = Path.Combine(path, filePath);
                Uploadfile(model.FilePath, fullPath);

                var data = new Document()
                {
                    FileName = model.FileName,
                   

                    FilePath = filePath,
                    Uid = 1,
                    Length = model.Length
                  
                };
                _context.Add(data);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public void Uploadfile(IFormFile file,string path)
        {
            FileStream stream =  new FileStream(path, FileMode.Create);
            file.CopyTo(stream);
        }
            //return Ok(new { count = files.Count, size });
          

    

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.UidNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            var path = Path.Combine(_env.WebRootPath, document.FilePath);
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            return File(memoryStream, "application/octet-stream", document.FilePath);
        }


        // GET: Documents/Create
        public IActionResult Create()
        {
            ViewData["Uid"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Uid,FileName,ContentType,FilePath,Length")] Document document)
        {
            if (ModelState.IsValid)
            {
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Uid"] = new SelectList(_context.Users, "Id", "Id", document.Uid=1);
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["Uid"] = new SelectList(_context.Users, "Id", "Id", document.Uid);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Uid,FileName,ContentType,FilePath,Length")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
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
            ViewData["Uid"] = new SelectList(_context.Users, "Id", "Id", document.Uid);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.UidNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'MultipleFileSystemContext.Documents'  is null.");
            }
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
          return _context.Documents.Any(e => e.Id == id);
        }
    }
}
