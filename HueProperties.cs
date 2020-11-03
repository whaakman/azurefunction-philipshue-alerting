namespace Alerting.Function
{    public class HueProperties
    {

        // By default bool is set to false, but we don't want to take a chance :)
        public bool on { get; set; } = false;
        public int bri { get; set; }  
        public int hue { get; set; }
        public int sat { get; set; }

        // Method to set the color depending on the parameter value provided. Returns the object
        public HueProperties SetColor(string color)
        {
            var properties = new HueProperties();
            
            if (color == "green")
            {
                properties.bri = 254;
                properties.hue = 23456;
                properties.sat = 254;
                properties.on = true;
            }

            if (color == "red")
            {
                properties.bri = 254;
                properties.hue = 908;
                properties.sat = 254;
                properties.on = true;
            }

            return properties;
        }
    }
}