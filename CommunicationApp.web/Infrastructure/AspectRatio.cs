using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommunicationApp.Infrastructure
{
    public class AspectRatio
    {
        public AspectRatio()
        {
        }
        private int d_Width = 0;
        private int d_Height = 0;
        public int Width
        {
            get { return d_Width; }
            set { d_Width = value; }
        }
        public int Height
        {
            get { return d_Height; }
            set { d_Height = value; }
        }
        /// <summary>
        /// Methord For Calculate Hight and Width
        /// </summary>
        /// <param name="aWidth"></param>
        /// <param name="aHeight"></param>
        /// <param name="dWidth"></param>
        /// <param name="dHeight"></param>
        public void WidthAndHeight(int aWidth, int aHeight, int dWidth, int dHeight)
        {
            double height = aHeight;
            double width = aWidth;
            double rWidht = Convert.ToDouble(dWidth);
            double rHeight = Convert.ToDouble(dHeight);
            int fWidth = 0;
            int fHeight = 0;
            double hRatio = 0.0;
            double vRatio = 0.0;
            if (width > rWidht)
            {
                hRatio = (rWidht / width);
                vRatio = (rHeight / height);

                if (vRatio > hRatio)
                {
                    fWidth = Convert.ToInt32((double)width * hRatio);
                    fHeight = Convert.ToInt32((double)height * hRatio);
                }
                else
                {
                    fWidth = Convert.ToInt32((double)width * vRatio);
                    fHeight = Convert.ToInt32((double)height * vRatio);
                }

            }
            else if (rWidht > width)
            {
                hRatio = (rWidht / width);
                vRatio = (rHeight / height);

                if (vRatio > hRatio)
                {
                    fWidth = Convert.ToInt32((double)width * hRatio);
                    fHeight = Convert.ToInt32((double)height * hRatio);
                }
                else
                {
                    fWidth = Convert.ToInt32((double)width * vRatio);
                    fHeight = Convert.ToInt32((double)height * vRatio);
                }
            }
            else if (height > rHeight)
            {
                hRatio = (rWidht / width);
                vRatio = (rHeight / height);

                if (vRatio > hRatio)
                {
                    fWidth = Convert.ToInt32((double)width * hRatio);
                    fHeight = Convert.ToInt32((double)height * hRatio);
                }
                else
                {
                    fWidth = Convert.ToInt32((double)width * vRatio);
                    fHeight = Convert.ToInt32((double)height * vRatio);
                }
            }
            else if (rHeight > height)
            {
                hRatio = (rWidht / width);
                vRatio = (rHeight / height);

                if (vRatio > hRatio)
                {
                    fWidth = Convert.ToInt32((double)width * hRatio);
                    fHeight = Convert.ToInt32((double)height * hRatio);
                }
                else
                {
                    fWidth = Convert.ToInt32((double)width * vRatio);
                    fHeight = Convert.ToInt32((double)height * vRatio);
                }
            }
            d_Width = fWidth;
            d_Height = fHeight;
        }
    }
}