using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// reference: http://nicholasarmstrong.com/2010/02/exif-quick-reference/

namespace TDF.Samples
{
    public class ExifMetadata
    {
        public ExifMetadata(Stream source)
        {
            using (var image = Image.FromStream(source))
            {
                //foreach (var item in image.PropertyItems)
                //{
                //    var info = string.Format("Id = {0}, Type = {1}", item.Id, item.Type);
                //    Trace.WriteLine(info);
                //}

                PropertyItem dimensionX = image.GetPropertyItem((int)ExifKey.DimensionPixelX);
                PropertyItem dimensionY = image.GetPropertyItem((int)ExifKey.DimensionPixelY);
                Dimension = new Size(
                    BitConverter.ToInt32(dimensionX.Value, 0),
                    BitConverter.ToInt32(dimensionY.Value, 0));
            }
        }

        public Size Dimension { get; private set; }

    }
}
