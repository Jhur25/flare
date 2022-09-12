using FlareExam.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

namespace FlareExam.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult RemoveRectangle(Param rect)
        {

            var newRec = rect.rects.Where(i => i.xaxis == rect.x && i.yaxis == rect.y && i.id != "myCanvas").FirstOrDefault();
            //var Grid = newRec.FirstOrDefault(i => i.id == "myCanvas");

            //var result = Service.Service.User.deleteUser(id);
            return Json(new GridResponse { isSuccess = true, result = "Success", rect = newRec, message = "Rectangle is Accepted." });

        }

        public IActionResult AddGrid(GridModel grid)
        {

            if (grid.xaxis < 0 || grid.yaxis < 0)
                return Json(new GridResponse { isSuccess = false, result = "Failed", message = "The axis not lesser than 0. " });

            if ((grid.width < 5 || grid.width > 25) || (grid.height < 5 || grid.height > 25))
                return Json(new GridResponse { isSuccess = false, result = "Failed", message = "The width or height not lesser than 5 or greater than 25. " });

            //var result = Service.Service.User.deleteUser(id);
            return Json(new GridResponse { isSuccess = true, result = "Success", grid = grid, message = "Successfully validated" });

        }
       
        [HttpPost]
        public IActionResult CheckRectangle(List<RectangleModel> rects)
        {

            var newRec = rects.DistinctBy(i => i.id).ToList();
            var Grid = newRec.FirstOrDefault( i => i.id == "myCanvas");

            if(newRec.Count == 2)
            {
                var rect = newRec.FirstOrDefault(i => i.id != "myCanvas");
                if (rect != null || Grid != null)
                {
                    var valid = ValidateRectangle(Grid, rect);
                    if(valid  != null)
                    {
                        return Json(valid);
                    }
                  
                }
            }
           else if (newRec.Count > 2)
            {
                var rect = newRec.Last(i => i.id != "myCanvas");
                var rectList = newRec.Where(i => i.id != "myCanvas").ToList();
                if (rect != null || Grid != null)
                {
                    var check = ValidateRectangle(Grid, rect);
                    if (check != null)
                    {
                        return Json(check);
                    }
                }

                var Rectangle = new RectangleModel();
                for (int i = 0; i < rectList.Count(); i++)
                {   
                    for (int t = 0; t < rectList.Count(); t++)
                    { 
                        var ctr = t + 1;
                        if (ctr < rectList.Count && i < ctr)
                        {
                                var res = IsOverLapped(rectList[i], rectList[ctr]);
                                if (res)
                                {
                                    return Json(new GridResponse { isSuccess = false, result = "Failed", message = "Overlapped Rectangles" });

                                }
                        }
                    }
                    
                }


            }

            //var result = Service.Service.User.deleteUser(id);
            return Json(new GridResponse { isSuccess = true, result = "Success", message = "Rectangle is Accepted." });

        }

        private GridResponse ValidateRectangle(RectangleModel Grid, RectangleModel rect)
        {
            var pwidth = 0;
            var pheight = 0;
            var px = 0;
            var py = 0;
            var tx = 0;
            var ty = 0;

            if (Grid != null)
            {
                pwidth = Grid.width;
                pheight = Grid.height;
                px = Grid.xaxis;
                py = Grid.yaxis;
                tx = Grid.width + Grid.xaxis;
                ty = Grid.height + Grid.yaxis;
            }

            if (rect != null || Grid != null)
            {
                var rx = rect.width + rect.xaxis;
                var ry = rect.height + rect.yaxis;

                if ((rect?.width > pwidth || rect?.width <= 0) || (rect?.height > pheight || rect?.height <= 0))
                {
                    return new GridResponse { isSuccess = false, result = "Failed", message = "Height and Weight must not exceed or less in Grid size." };

                }
                if ((rect?.xaxis < px || rect?.xaxis > tx) || (rect?.yaxis < py || rect?.yaxis > ty) )
                {
                   return  new GridResponse { isSuccess = false, result = "Failed", message = "X axis and Y axis must not exceed or less in Grid position." };

                }
                if ((rx > tx || ry > ty))
                {
                    return new GridResponse { isSuccess = false, result = "Failed", message = " X axis and Width or Y axis and Height must not lapse on Grid." };
                }


            }
            else {
                return new GridResponse { isSuccess = false, result = "Failed", message = "Height and Weight must not exceed or less in Grid size." };
            }


            return null;
        }

        public static bool IsOverLapped(RectangleModel r1, RectangleModel r2)
        {
            int x1 = Math.Max(r1.xaxis, r2.xaxis);
            int x2 = Math.Min(r1.xaxis + r1.width, r2.xaxis + r2.width);
            int y1 = Math.Max(r1.yaxis, r2.yaxis);
            int y2 = Math.Min(r1.yaxis + r1.height, r2.yaxis + r2.height);

            return (x2 > x1 && y2 > y1);
        }
    }
}