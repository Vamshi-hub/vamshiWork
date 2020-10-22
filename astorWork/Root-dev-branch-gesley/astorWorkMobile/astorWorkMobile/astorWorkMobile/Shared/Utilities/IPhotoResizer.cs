using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkMobile.Shared.Utilities
{
    public interface IPhotoResizer
    {
        Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality);
    }
}
