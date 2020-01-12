namespace HttpReports.Dashboard.Models
{
    public class EchartPineDataModel
    {
        public EchartPineDataModel()
        {
        }

        public EchartPineDataModel(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public int Value { get; set; }
    }
}