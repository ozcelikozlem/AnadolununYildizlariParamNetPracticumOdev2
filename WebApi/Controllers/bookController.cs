using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.DbOperations;
namespace WebApi.AddControllers
    {
        [ApiController]
        [Route("api/[controller]s")]
        public class bookController : ControllerBase
        {
            private readonly BookStoreDbContext _context;
            public bookController(BookStoreDbContext context)
            {
                _context = context;
            }
            
            [HttpGet] // GET: api/books
            public List<Book> GetBooks()
            {
                var bookList = _context.Books.OrderBy(x=>x.Id).ToList<Book>();
                return bookList;
            }

            /*[HttpGet("lastadded")] // GET: api/books/lastadded
            public List<Book> GetBooksLast()
            {
                var bookListDesc = _context.Books.OrderByDescending(x=>x.Id).ToList<Book>();
                return bookListDesc;
            }*/
            
            [HttpGet("alphabetical")] // GET: api/books/alphabetical
            public List<Book> GetBooksAlphabetical()
            {
                var bookListAlphabetical = _context.Books.OrderBy(x=>x.Title).ToList<Book>();
                return bookListAlphabetical;
            }

            [HttpGet("newedition")] // GET: api/books/newedition
            public List<Book> GetBooksEdition()
            {
                var bookListEdition = _context.Books.OrderByDescending(x=>x.PublishDate).ToList<Book>();
                return bookListEdition;
            }

            [HttpGet("{id}")] // GET: api/books/id
            public Book GetBookById(int id)
            {
                var book = _context.Books.Where(book=>book.Id==id).SingleOrDefault();
                if(book is null)
                {
                    throw new Exception("Kitap mevcut değil.");
                }
                return book;
            }

            [HttpGet] //GET:api/books/getquery
            [Route("getquery")]
            public Book Get([FromQuery] string id)
            {
                var book=_context.Books.Where(book=>book.Id==Convert.ToInt32(id)).SingleOrDefault();
                return book;
            }
            
            [HttpGet("list")] //GET:api/books/list?genreId=id
            public IEnumerable<Book> GetProductsByCategory(string genreId)
            {
                var bookList = _context.Books.OrderBy(x=>x.Id).ToList<Book>();
                return bookList.Where(
                p => string.Equals(p.GenreId.ToString(), genreId, StringComparison.OrdinalIgnoreCase));
            }

            [HttpPost]// POST:api/books
            public IActionResult AddBook([FromBody] Book newBook)
            {
                var book =_context.Books.SingleOrDefault(x=> x.Title==newBook.Title);
                if(book is not null)
                {
                    throw new Exception("Kitap mevcut.");
                }
                _context.Books.Add(newBook);
                _context.SaveChanges();
                return Ok("Book Registration Created");
            }

            [HttpPut("{id}")]//PUT [FromBody] : api/books/id
            public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
            {
                var book =_context.Books.SingleOrDefault(x=> x.Id==id);
                if(book is null)
                {
                    throw new Exception("Kitap mevcut değil.");
                }
                
                book.GenreId=updatedBook.GenreId != default ? updatedBook.GenreId : book.GenreId;
                book.PageCount= updatedBook.PageCount != default ? updatedBook.PageCount : book.PageCount;
                book.PublishDate= updatedBook.PublishDate != default ? updatedBook.PublishDate : book.PublishDate;
                book.Title=updatedBook.Title != default ? updatedBook.Title : book.Title;
                _context.SaveChanges();
                return Ok("Book Registration Updated");
                
            }

            // [HttpPut]//PUT [FrmomQuery]:: api/books/updatebookquery
            // [Route("updatebookquery")]
            // public IActionResult UpdateBook([FromQuery] string  id, [FromBody] Book updatedBook)
            // {
            //     var book =_context.Books.SingleOrDefault(x=> x.Id==Convert.ToInt32(id));
            //     // if(book is null)
            //     // {
            //     //     return BadRequest();
            //     // }
            //     book.GenreId=updatedBook.GenreId != default ? updatedBook.GenreId : book.GenreId;
            //     book.PageCount= updatedBook.PageCount != default ? updatedBook.PageCount : book.PageCount;
            //     book.PublishDate= updatedBook.PublishDate != default ? updatedBook.PublishDate : book.PublishDate;
            //     book.Title=updatedBook.Title != default ? updatedBook.Title : book.Title;
            //     _context.SaveChanges();
            //     return Ok("Book Registration Updated");
            // }

            [HttpDelete("{id}")]//DELETE [FromBody] :: api/books/id
            public IActionResult DeleteBook(int id)
            {
                var book=_context.Books.SingleOrDefault(x=>x.Id==id);
                if (book is null)
                {
                    throw new Exception("Kitap mevcut değil.");
                }
                _context.Books.Remove(book);
                _context.SaveChanges();
                return Ok("Book Registration Deleted");
            }

            // [HttpDelete]//DELETE [FromQuery] : api/books/deletebookquery
            // [Route("deletebookquery")]
            // public IActionResult DeleteBook([FromQuery] string  id)
            // {
            //     var book=_context.Books.SingleOrDefault(x=>x.Id==Convert.ToInt32(id));
            //     // if (book is null)
            //     // {
            //     //     return BadRequest();
            //     // }
            //     _context.Books.Remove(book);
            //     _context.SaveChanges();
            //     return Ok("Book Registration Updated");
            // }

            /*op: “add”, “remove”, “replace”, “move”, “copy” ve “test”.
            [
            {
            "op":"replace",
            "path":"/players/2/nationality",
            "value":"Hırvatistan"
            }
            ]*/
            [HttpPatch("{id}")] // PATCH [FromBody]: api/books/id
            public IActionResult PatchBook(int id,JsonPatchDocument<Book> patchBook)
            {
                if (patchBook == null)
                {
                    return BadRequest(ModelState);
                }
                 var book =_context.Books.SingleOrDefault(x=> x.Id==id);
                if(book is null)
                {
                    throw new InvalidOperationException("Kitap mevcut değil.");
                }
                patchBook.ApplyTo(book,ModelState);
                if (ModelState.IsValid)
                {
                    _context.SaveChanges();
                    return Ok(book);
                }
                return BadRequest(ModelState);
            }

            // [HttpPatch] // PATCH [FromQuery]: api/books/patchbookquery
            // [Route("patchbookquery")]
            // public IActionResult PatchBook([FromQuery] string id,JsonPatchDocument<Book> patchBook)
            // {
            //     // if (patchBook == null)
            //     // {
            //     //     return BadRequest(ModelState);
            //     // }
            //     var book =_context.Books.SingleOrDefault(x=> x.Id==Convert.ToInt32(id));
            //     // if(book is null)
            //     // {
            //     //     return NotFound();
            //     // }
            //     patchBook.ApplyTo(book,ModelState);
            //     if (ModelState.IsValid)
            //     {
            //         _context.SaveChanges();
            //         return Ok(book);
            //     }
            //     return BadRequest(ModelState);
            // }
        }
    }