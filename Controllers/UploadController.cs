using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dynastream.Fit;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace road_scholar.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        const int LATLONGOFFSET = 7;
        const string LATITUDE = "PositionLat";
        const string LONGITUDE = "PositionLong";
        const string SPEED = "Speed";
        const string HEARTRATE = "HeartRate";
        const string TIMESTAMP = "Timestamp";
        const string DISTANCE = "Distance";

        static Dictionary<ushort, int> mesgCounts = new Dictionary<ushort, int>();
        static Route route = new Route();

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //Use this function to upload GPS files.
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "FIT");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    //Save file to Resources/Fit
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //Process the uploaded fit file
                    ProcessFitFile(fullPath);
                    JsonConvert.SerializeObject(route.getCoordinatePoints());

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        //This function adds coordinate points to the global list
        //These points will be displayed on the map.
        static void OnMesg(object sender, MesgEventArgs e)
        {
            try
            {
                CoordinatePoint coordinatePoint = new CoordinatePoint();
                foreach (Field field in e.mesg.Fields)
                {
                    for (int i = 0; i < field.GetNumValues(); i++)
                    {
                        //Use switch statement to set value on the coordinatePoint.
                        switch (field.GetName())
                        {
                            case LATITUDE:
                                coordinatePoint.latitude = reformatLatLong(field.GetValue(i).ToString());
                                break;

                            case LONGITUDE:
                                coordinatePoint.longitude = reformatLatLong(field.GetValue(i).ToString());
                                break;

                            case TIMESTAMP:
                                coordinatePoint.timeStamp = double.Parse(field.GetValue(i).ToString());
                                break;

                            case DISTANCE:
                                coordinatePoint.distance = double.Parse(field.GetValue(i).ToString());
                                break;

                            case SPEED:
                                coordinatePoint.speed = double.Parse(field.GetValue(i).ToString());
                                break;

                            case HEARTRATE:
                                coordinatePoint.heartRate = double.Parse(field.GetValue(i).ToString());
                                break;

                            default:
                                //do nothing :)
                                break;
                        }
                    }
                }
                if (coordinatePoint.HasLatLong())
                    route.addCoordinatePoint(coordinatePoint);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error parsing double for JSON." + ex.Message);
            }
        }

        static private void ProcessFitFile(string fileName)
        {
            try
            {
                Decode decodeFit = new Decode();
                FileStream fitSource = new FileStream(fileName, FileMode.Open);
                // Attempt to open .FIT file

                Decode decodeDemo = new Decode();
                MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();

                // Connect the Broadcaster to our event (message) source (in this case the Decoder)
                decodeDemo.MesgEvent += OnMesg;

                bool status = decodeDemo.IsFIT(fitSource);
                status &= decodeDemo.CheckIntegrity(fitSource);

                // Process the file
                if (status)
                {
                    decodeDemo.Read(fitSource);
                }
                else
                {
                    try
                    {
                        if (decodeDemo.InvalidDataSize)
                            decodeDemo.Read(fitSource);
                        else
                            decodeDemo.Read(fitSource, DecodeMode.InvalidHeader);
                    }
                    catch (FitException ex)
                    {
                    }
                }
                fitSource.Close();

                int totalMesgs = 0;
                foreach (KeyValuePair<ushort, int> pair in mesgCounts)
                    totalMesgs += pair.Value;
            }
            catch (FitException ex)
            {
                Console.WriteLine("A FitException occurred when trying to decode the FIT file. Message: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred when trying to decode the FIT file. Message: " + ex.Message);
            }
        }

        //This function adds a decimal place to the latitude and longitude strings if needed.
        static double reformatLatLong(string coord)
        {
            try
            {
                double semicircleValue = double.Parse(coord);

                //Convert coordinate value from semicircle to lat/long value
                //Formula is (180 / 2^31) * semicircle value = degrees
                return (180 / (Math.Pow(2, 31))) * semicircleValue;
                
            }
            catch(Exception e)
            {
                return 0;
            }
        }
    }
}
