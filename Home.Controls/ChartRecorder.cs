using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Home.Services
{
    public class ChartRecorder
    {
        public Dictionary<string, ChartSeries> SeriesList { get; set; }
        /// <summary>
        /// Max number of records per series
        /// Default: 5000
        /// </summary>
        public int MaxSeriesRecords {get; set;}

        public ChartRecorder()
        {
            SeriesList = new Dictionary<string, ChartSeries>();
            MaxSeriesRecords = 5000;
        }

        public void AddData(string seriesLabel, double y)
        {
            AddData(seriesLabel, DateTime.Now, y);
        }

        public void AddData(string seriesLabel, DateTime x, double y)
        {
            if(SeriesList.ContainsKey(seriesLabel) == false)
            {
                // make new series for label
                ChartSeries nSeries = new ChartSeries()
                {
                    Label = seriesLabel,
                };
                SeriesList.Add(seriesLabel, nSeries);
            }
            else
            {
                // check max hasn't been reached and make space if one has.
                if(SeriesList[seriesLabel].Data.Count == 5000)
                {
                    SeriesList[seriesLabel].Data.Remove(SeriesList[seriesLabel].Data.First());
                }
            }

            SeriesList[seriesLabel].AddPoint(new ChartDataPoint() { x = x, y = y});

            //AppContext.WriteLine("RECORDER: {0} now has {1} points", seriesLabel, SeriesList[seriesLabel].Data.Count);
        }

        public List<ChartJsSeries> GetJsSeriesList(int? MaxRecords = null)
        {
            List<ChartJsSeries> list = new List<ChartJsSeries>();
            foreach (ChartSeries series in SeriesList.Values)
            {
                list.Add(series.ToJsSeries(MaxRecords));
            }
            return list;
        }
    }

    public class ChartSeries
    {
        private static DateTime JsMinTime = new DateTime(1970,1,1);
        public string Label { get; set; }
        /// <summary>
        /// x and y value pairs
        /// </summary>
        public List<ChartDataPoint> Data { get; set; }

        public ChartSeries()
        {
            Data = new List<ChartDataPoint>();
        }

        public void AddPoint(ChartDataPoint point)
        {
            Data.Add(point);
            return;

            if(Data.Count == 0)
            {
                Data.Add(point);
            }
            else if(Data.Last().x > point.x)
            {
                // need to find where to insert in list
                // first check if new point can go at begining
                if(Data.First().x > point.x)
                {

                }
            }
        }

        public ChartJsSeries ToJsSeries(int? MaxRecords = null)
        {
            List<double[]> data = new List<double[]>();

            var orgData = Data.ToList();
            if (MaxRecords.HasValue)
            {
                orgData.Reverse();
                orgData = orgData.Take(MaxRecords.Value).ToList();
                orgData.Reverse();
            }

            foreach (var pnt in orgData)
            {
                data.Add(NewJsDataPoint(pnt.x, pnt.y));
            }
            return new ChartJsSeries()
            {
                data = data,
                label = Label
            };
        } 
        
        public static double[] NewJsDataPoint(DateTime x, double y)
        {
            double xTime = (x - JsMinTime).TotalSeconds * 1000;
            return new double[] { xTime, y };
        }
    }

    public class ChartDataPoint
    {
        public DateTime x { get; set; }
        public double y { get; set; }
    }

    public class ChartJsSeries
    {
        public string label { get; set; }
        /// <summary>
        /// x and y value pairs
        /// </summary>
        public List<double[]> data { get; set; }

        public void AddPoint(double x, double y)
        {
            data.Add(NewData( x, y));
        }

        public static double[] NewData(double x, double y)
        {
            return new double[] { x, y };
        }
    }
}
