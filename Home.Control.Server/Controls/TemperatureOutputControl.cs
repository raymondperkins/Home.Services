using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Raspberry.IO.GeneralPurpose;

using Home.Services;

namespace Home.Control.Server
{
    public class TemperatureOutputControlConfig : IOControlConfig
    {
        private string _gpio;
        private List<ProcessorPin> _pin;

        public string TempSensorId {get; set;}
        public double Temperature { get; set; }
        public double TemperatureSetpoint { get; set; }
        public double TemperatureMargin { get; set; }
        public TimeSpan RecordInterval { get; set; }

        public new string GPIO 
        {
            get { return _gpio; } 
            set 
            {
                _pin = new List<ProcessorPin>();
                var pins = value.Split(new char[] { ' ' });
                foreach (var p in pins)
                    _pin.Add(this.ConvertStringToPin(p));
            }
        }

        public new List<ProcessorPin> Pin { get { return _pin; } }
    }

    public class TemperatureOutputControl : Control<TemperatureOutputControlConfig>
    {
        bool isEnabled = false;
        TemperatureOutputControlConfig config = null;

        private Task temperatureTask;
        private bool pinStates = false;
        private DateTime lastRecordTime = DateTime.MinValue;

        public TemperatureOutputControl(TemperatureOutputControlConfig Config)
        {
            config = Config;

            //RecordInterval = new TimeSpan(0, 0, 5); // default 15 seconds minute
            //Recorder = new ChartRecorder();
            //TemperatureMargin = 2.0;
           // Name = string.Format("{0} temp controller", TempId);
            //tempSensorId = TempId;
            // setup each pin
            //Pins = gpioPins.ToList();
            foreach (var outpin in Config.Pin)
            {
                GpioConnectionSettings.DefaultDriver.Allocate(outpin, PinDirection.Output);
                App.Debug("Controls: Allocated output for {0}", outpin);
                //outpin.SetPinAsDisabled();
                //State = State || outpin.Read();
            }
            DisableAllPins();

            
        }

        public override void SetControlValue(bool value)
        {
            isEnabled = value;
            if (value)
            {
                // start temperature task

            }
        }

        public override bool GetControlValue()
        {
            // controller is active is the controller task is running
            return temperatureTask == null ? false : temperatureTask.IsCompleted == false;
        }

        public override TemperatureOutputControlConfig GetControlConfig()
        {
            return config;
        }

        public override void SetControlConfig(TemperatureOutputControlConfig value)
        {
            //throw new Exception("This config is not allowed to be changed");
        }

        private void SetControllerState(bool state)
        {
            if (state == true)
            {
                if (isEnabled == false)
                {
                    StopController();
                    // start task
                    isEnabled = true;
                    temperatureTask = Task.Factory.StartNew(() => ControllerTask());
                }
            }
            else
            {
                StopController();
            }
        }

        private void StopController()
        {
            if (temperatureTask != null)
            {
                // start task
                isEnabled = false;
                if (temperatureTask.Status == TaskStatus.Running)
                {
                    if (temperatureTask.Wait(1500) == false)
                    {
                        throw new Exception("TEMPCONTROLLER: Task failed to stop");
                    }
                }
            }
        }

        private void ControllerTask()
        {
            DateTime lastSensorRead = DateTime.MinValue;
            string sensorPath = Path.Combine("/sys/bus/w1/devices/", config.TempSensorId, "w1_slave");

            App.Debug("TEMPCONTROLLER: Task starting");

            while (isEnabled)
            {
                // check temp sesnor is still there
                if (File.Exists(sensorPath) == false)
                {
                    throw new Exception("TEMPCONTROLLER: Temp sensor not found, quiting temperature control");
                }

                if ((DateTime.Now - lastSensorRead).TotalSeconds > 5)
                {
                    // read and print sensor
                    //AppContext.WriteLine("TEMPCONTROLLER: Reading temp sensor file {0}:", sensorPath);
                    string[] tempLines = File.ReadAllLines(sensorPath);
                    lastSensorRead = DateTime.Now;
                    //foreach (string line in tempLines)
                    //    AppContext.WriteLine(line);
                    double? readTemp = null;
                    if (tempLines.Length == 2)
                    {
                        int tempInd = tempLines[1].IndexOf("t=");
                        if (tempInd >= 0)
                        {
                            tempInd += 2;
                            string str = tempLines[1].Substring(tempInd, tempLines[1].Length - tempInd);
                            double temp;
                            if (double.TryParse(str, out temp) == true)
                            {
                                readTemp = temp / 1000;

                            }
                        }
                    }
                    
                    if (readTemp.HasValue)
                    {
                        config.Temperature = readTemp.Value;
                        //UpdateDescription();
                        if (config.TemperatureSetpoint > (readTemp.Value + config.TemperatureMargin))
                        {
                            // temperature is below setpoint
                            EnableAllPins();
                        }
                        else if (config.TemperatureSetpoint < (readTemp.Value - config.TemperatureMargin))
                        {
                            // temperature is above setpoint
                            DisableAllPins();
                        }
                        App.Debug("TEMPCONTROLLER: Temp is: {0}°C, Setpoint: {1}°C, Margin: {2}°C, Output:{3}", readTemp, config.TemperatureSetpoint, config.TemperatureMargin, pinStates);
                    }

                    if (DateTime.Now > (lastRecordTime + config.RecordInterval))
                    {
                        AddRecord();
                    }
                }

                System.Threading.Thread.Sleep(500);
            }

            DisableAllPins();
            App.Debug("TEMPCONTROLLER: Task exiting");
        }

        private void DisableAllPins()
        {
            WriteAllPins(false);
        }

        private void EnableAllPins()
        {
            WriteAllPins(true);
        }

        private void WriteAllPins(bool value)
        {
            App.Debug("TEMPCONTROLLER: {0}:{1}", "Setting all pins", value);
            foreach (var outpin in config.Pin)
            {
                // set pin to off
                try
                {
                    GpioConnectionSettings.DefaultDriver.Write(outpin, config.Inverted ? !value : value);
                    //pinStates = true;
                }
                catch (Exception ex)
                {
                    App.Debug("TEMPCONTROLLER: {0}", ex.ToString());
                }
            }

        }

        private void AddRecord()
        {
            /*
            if (Recorder != null)
            {
                lastRecordTime = DateTime.Now;
                // record values ser: setpoint, output value, plant value
                DateTime timeNow = DateTime.Now;
                Recorder.AddData("Plant", timeNow, Temperature);
                Recorder.AddData("Setpoint", timeNow, TemperatureSetpoint);
                Recorder.AddData("Control", timeNow, pinStates ? 100.0 : 0.0);
                // should also show individual output pin states :
                foreach (var outpin in Pins)
                {
                    Recorder.AddData(string.Format("Output{0}", outpin.Pin), timeNow, outpin.Read() ? 100.0 : 0.0);
                }
            }*/
        }
    }
}
