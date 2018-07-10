using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// reference: http://nicholasarmstrong.com/2010/02/exif-quick-reference/

namespace TDF.Samples
{
    public enum ExifKey
    {
        EquipmentMaker = 271,   // [ASCII string]   ‘Canon’, ‘Nikon’, etc.
        EquipmentModel = 272,   // [ASCII string]   ‘Canon PowerShot S5 IS’, etc.
        EquipmentMakerNote = 37500, // Camera Maker specific information
        FileSource = 41728,     // [int]            1 = Film scanner, 2 = Reflection print scanner, 3 = Digital camera
        Orientation = 274,      // [ushort]         1 = Horizontal, 3 = Rotate 180 degrees, 6 = Rotate 90 degrees clockwise, 8 = Rotate 270 degrees clockwise
        ResolutionX = 282,      // [ushort]         Unit in Resolution Unit field (for pixels, see Pixel X Dimension field)
        ResolutionY = 283,      // [ushort]         Unit in Resolution Unit field (for pixels, see Pixel X Dimension field)
        DimensionPixelX = 40962,// [ushort]         In pixels
        DimensionPixelY = 40963,// [ushort]         In pixels
        ResolutionUnit = 296,   // [ushort]         1 = None, 2 = Inches, 3 = Centimeters
        DateTaken = 36867,      // [ASCII string]   YYY:MM:DD HH:MM:SS
        DateCreated = 36868,    // [ASCII string]   YYY:MM:DD HH:MM:SS
        DateTimeModified = 306, // [ASCII string]   YYYY:MM:DD HH:MM:SS
        ExposureTime = 33434,   // [uint,uint]      First integer divided by the second integer produces the exposure time in seconds; for example, a value of ’1′ followed by a value of ’50′ is an exposure time of 1/50th of a second.
        FNumber = 33437,        // [uint,uint]      First integer divided by the second integer produces the F number; for example, a value of ’35′ followed by a value of ’10′ is F/3.5.
        FocalLength = 37386,    // [uint,uint]      
        ISO = 34855,            // [ushort]         100, 200, 400, etc.
        ShutterSpeed = 37377,   // [int,int]
        Aperture = 37378,       // [uint,uint] 
        MaximumAperature = 37381,    // [uint,uint]  
        ExposureCompensation = 37380,// [int,int]   First integer divided by the second integer produces the exposure compensation; for example, a value of ’2′ followed by a value of ’3′ is +2/3
        MeteringMode = 37383,   // [ushort]         0 = Unknown, 1 = Average, 2 = Center-weighted average, 3 = Spot, 4 = Multi-spot, 5 = Multi-segment, 6 = Partial, 255 = Unknown 
        Flash = 37385,          // [ushort]         0 = No Flash, LSB (8th bit) set = Flash Fired, bits 4&5, L-R:, 10 = Flash off, 01 = Flash on, 11 = Flash auto
        ColorSpace = 40961,     // [ushort]         1 = sRGB

    }
}
