namespace FlareExam.Models
{
    public class GridResponse
    {
        public string result { get; set; } = "";
        public bool isSuccess { get; set; }
        public string message { get; set; } = "";

        public GridModel grid { get; set; } = new GridModel();
        public RectangleModel rect { get; set; } = new RectangleModel();
    }
}
