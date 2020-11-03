# Azure Function Philips Hue Alert
![](flash.gif)

This function sends an call to the Philips Hue bridge to flash the lights in the color "Green" or "Red" for the "count" amount of times.

### Requirements
The Hue bridge needs to be accessible from the Azure Function. This includes potential port forwarding to your Hue Bridge and a user on your Hue Bridge. Follow these steps to create a user: https://developers.meethue.com/develop/get-started-2/

Please be careful exposing your Hue Bridge to the internet without additional security configurations in your network. 

### Configuration
The URI for the specific light you want to target can be configured in the app settings like so:
Key: "hueURL"
Value: "_http:///<ADDRESS\>/api/<USER\>/lights/<Light\>/state_"

Light currently flashes Red or Green. Additional colors can be introduced by adding them to __HueProperties.cs__.
Example:

```C#
    if (color == "purple")
    {
        properties.bri = 254;
        properties.hue = 48186;
        properties.sat = 254;
        properties.on = true;
    }
```
Additionally the brightness can be adjusted by changing the value of _properties.bri_

Function is currently set to function level authentication. Can change if needed. Requests are captured from the GET request as this allows for easy integration with Azure Services such as Action Groups. 

### Usage 
Function can be leveraged through _http:///<FunctionAppURI\>/api/TriggerAlertLighting?alert=<COLOR\>&count=<COUNT\>&code=<FUNCTIONKEY\>_

Lights should start flashing pretty quickly :)