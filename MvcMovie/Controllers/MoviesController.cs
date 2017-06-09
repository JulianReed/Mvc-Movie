using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcMovie.Models;
using System.IO;
using Newtonsoft.Json;


namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private MvcMovieContext db = new MvcMovieContext();

        // GET: Movies


        //public ActionResult Index(string sortOrder)
        //{
        //    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "iRating_desc" : "";
        //    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
        //    var movies = from s in db.Movies
        //                   select s;
        //    switch (sortOrder)
        //    {
        //        case "iRating_desc":
        //            movies = movies.OrderByDescending(s => s.iRating);
        //            break;
        //    }
        //    return View(movies.ToList());
        //}



        //public ViewResult Index(string sortOrder, string searchString)
        //{
        //    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
        //    var students = from s in db.Students
        //                   select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        students = students.Where(s => s.LastName.Contains(searchString)
        //                               || s.FirstMidName.Contains(searchString));
        //    }
        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            students = students.OrderByDescending(s => s.LastName);
        //            break;
        //        case "Date":
        //            students = students.OrderBy(s => s.EnrollmentDate);
        //            break;
        //        case "date_desc":
        //            students = students.OrderByDescending(s => s.EnrollmentDate);
        //            break;
        //        default:
        //            students = students.OrderBy(s => s.LastName);
        //            break;
        //    }

        //    return View(students.ToList());
        //}




        public ActionResult Index(string movieGenre, string searchString, string directorString, string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "iRating_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";


            var GenreLst = new List<string>();

            var GenreQry = from d in db.Movies
                           orderby d.Genre
                           select d.Genre;

            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.movieGenre = new SelectList(GenreLst);

            var movies = from m in db.Movies
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }

            if (!string.IsNullOrEmpty(directorString))
            {
                movies = movies.Where(x => x.Director.Contains(directorString));
            }


            movies = movies.OrderByDescending(s => s.iRating);

            //return View(movies.ToList());
            return View(movies);
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,ReleaseDate,Genre,Price,Rating,Director,iRating,PosterURL")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ReleaseDate,Genre,Price,Rating,Director,iRating,PosterURL")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string getPosterURL(int? id)
        {
            Movie movie = db.Movies.Find(id);
            var plusTitle = movie.Title.Replace(" ", "+");
            System.Diagnostics.Debug.WriteLine(plusTitle);
            //var webRequest = WebRequest.Create(@"https://api.themoviedb.org/3/search/movie?api_key=efd3636a73f29caea4f872a3b84518f8&query=" + plusTitle);
            //var response = webRequest.GetResponse();
            //System.Diagnostics.Debug.WriteLine(response);
            //var content = response.GetResponseStream();
            //var reader = new StreamReader(content);

            var json = new WebClient().DownloadString("https://api.themoviedb.org/3/search/movie?api_key=efd3636a73f29caea4f872a3b84518f8&query="+plusTitle);
            //List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
            RootObject parsedJson = JsonConvert.DeserializeObject<RootObject>(json);
            System.Diagnostics.Debug.WriteLine("total results: " + parsedJson.total_results);
            var imageString = "https://image.tmdb.org/t/p/w500" + parsedJson.results[0].poster_path;



            //make this better w/ json file

            //var strContent = reader.ReadToEnd();
            //System.Diagnostics.Debug.WriteLine("Here's the file: " + strContent);
            //var jpgIndex = strContent.IndexOf(".jpg");
            //var posterPath = strContent.Substring(jpgIndex - 27, 27);

            //System.Diagnostics.Debug.WriteLine("Here's the string: " + posterPath);
            //var imageStr = "https://image.tmdb.org/t/p/w500/" + posterPath + ".jpg";
            return imageString;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class Result
    {
        public int vote_count { get; set; }
        public int id { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public string title { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public List<object> genre_ids { get; set; }
        public string backdrop_path { get; set; }
        public bool adult { get; set; }
        public string overview { get; set; }
        public string release_date { get; set; }
    }

    public class RootObject
    {
        public int page { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
        public List<Result> results { get; set; }
    }
}
