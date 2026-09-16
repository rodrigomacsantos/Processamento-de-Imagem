using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.IO;
using System.Linq;
using static System.Resources.ResXFileRef;
using System.Windows.Forms;
using System.Drawing;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Windows.Forms.VisualStyles;

namespace CG_OpenCV
{
    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        /// 
        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                // Acesso direto à memória da imagem (sequencial)
                // Direção: topo esquerdo -> fundo direito

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Apontador para a imagem original

                MIplImage imagem2 = imgCopy.MIplImage;
                byte* dataPtr1 = (byte*)imagem2.imageData.ToPointer(); // Apontador para a cópia da imagem
                byte blue, green, red;
                int x0, y0;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // número de canais - 3
                int padding = m.widthStep - m.nChannels * m.width; // bytes de alinhamento (padding)
                int x, y;

                if (nChan == 3) // imagem em RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            x0 = x - dx; // cálculo da nova coordenada x
                            y0 = y - dy; // cálculo da nova coordenada y
                            if (x0 >= 0 && y0 >= 0 && x0 < width && y0 < height) // verificando se as coordenadas estão dentro da imagem
                            {
                                // Copiando os valores dos pixels da imagem original para a nova posição na imagem traduzida
                                blue = (byte)(dataPtr1 + y0 * m.widthStep + x0 * nChan)[0];
                                green = (byte)(dataPtr1 + y0 * m.widthStep + x0 * nChan)[1];
                                red = (byte)(dataPtr1 + y0 * m.widthStep + x0 * nChan)[2];
                            }
                            else
                            {
                                // Preenchendo com preto caso o pixel esteja fora dos limites
                                blue = 0;
                                green = 0;
                                red = 0;
                            }

                            // Atualizando a imagem original com os novos valores de pixel
                            dataPtr[0] = blue;
                            dataPtr[1] = green;
                            dataPtr[2] = red;

                            dataPtr += nChan; // Avançar para o próximo pixel
                        }

                        dataPtr += padding; // Avançar para a próxima linha (considerando o padding)
                    }
                }
            }
        }

        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage imagem2 = imgCopy.MIplImage;
                byte* dataPtr1 = (byte*)imagem2.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;


                float radians = angle * (float)(Math.PI / 180.0);


                float cosTheta = (float)Math.Cos(radians);
                float sinTheta = (float)Math.Sin(radians);


                int centerX = width / 2;
                int centerY = height / 2;


                byte blue = 0, green = 0, red = 0;
                int x, y, x0, y0;

                if (nChan == 3)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {

                            int xRel = x - centerX;
                            int yRel = y - centerY;


                            x0 = (int)Math.Round((x - width / 2.0) * Math.Cos(angle) - (height / 2.0 - y) * Math.Sin(angle) + width / 2.0);
                            y0 = (int)Math.Round(height / 2.0 - (x - width / 2.0) * Math.Sin(angle) - (height / 2.0 - y) * Math.Cos(angle));



                            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                            {

                                byte* pixelPtr = dataPtr1 + y0 * widthStep + x0 * nChan;
                                blue = pixelPtr[0];
                                green = pixelPtr[1];
                                red = pixelPtr[2];
                            }
                            else
                            {

                                blue = 0;
                                green = 0;
                                red = 0;
                            }


                            byte* resultPtr = dataPtr + y * widthStep + x * nChan;
                            resultPtr[0] = blue;
                            resultPtr[1] = green;
                            resultPtr[2] = red;
                        }
                    }
                }
            }
        }


        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                // Acesso direto à memória da imagem (sequencial)
                // Direção: topo esquerdo -> fundo direito

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage imagem2 = imgCopy.MIplImage;
                byte* dataPtr1 = (byte*)imagem2.imageData.ToPointer();
                byte blue, green, red;
                int x0, y0;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int x, y;
                double hlf_w = width / 2.0;
                double hlf_h = height / 2.0;

                if (nChan == 3)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            x0 = (int)Math.Round(((x - hlf_w) / scaleFactor + width));

                            y0 = (int)Math.Round(((y - hlf_h) / scaleFactor + height));

                            if (x0 >= 0 && y0 >= 0 && x0 < width && y0 < height)
                            {

                                blue = (byte)(dataPtr1 + y0 * m.widthStep + x0 * nChan)[0];
                                green = (byte)(dataPtr1 + y0 * m.widthStep + x0 * nChan)[1];
                                red = (byte)(dataPtr1 + y0 * m.widthStep + x0 * nChan)[2];
                            }
                            else
                            {

                                blue = 0;
                                green = 0;
                                red = 0;
                            }


                            dataPtr[0] = blue;
                            dataPtr[1] = green;
                            dataPtr[2] = red;

                            dataPtr += nChan;
                        }

                        dataPtr += padding;
                    }
                }
            }
        }


        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;
                int x0, y0;
                int x, y;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)

                int hlf_w = width / 2;
                int hlf_h = height / 2;

                if (scaleFactor != 0) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            x0 = (int)Math.Round(((x - hlf_w) / scaleFactor) + centerX);
                            y0 = (int)Math.Round(((y - hlf_h) / scaleFactor) + centerY);

                            if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                            {
                                blue = (byte)(dataPtr2 + y0 * m2.widthStep + x0 * nChan)[0];
                                green = (byte)(dataPtr2 + y0 * m2.widthStep + x0 * nChan)[1];
                                red = (byte)(dataPtr2 + y0 * m2.widthStep + x0 * nChan)[2];


                                // store in the image
                                dataPtr[0] = blue;
                                dataPtr[1] = green;
                                dataPtr[2] = red;

                            }
                            else
                            {
                                dataPtr[0] = 0;
                                dataPtr[1] = 0;
                                dataPtr[2] = 0;

                            }

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }


        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Ponteiro para a imagem
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // número de canais (RGB = 3)
                int padding = m.widthStep - nChan * width; // bytes de alinhamento (padding)

                // Iterar sobre todos os pixels da imagem
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Acessar os canais RGB do pixel
                        for (int channel = 0; channel < 3; channel++) // 0: Blue, 1: Green, 2: Red
                        {
                            // Aplicar a fórmula para ajustar brilho e contraste

                            int pixelValue = (int)Math.Round(contrast * dataPtr[channel] + bright); // fórmula ajustada

                            // Limitar o valor do pixel ao intervalo [0, 255]
                            if (pixelValue < 0) pixelValue = 0;
                            if (pixelValue > 255) pixelValue = 255;

                            // Atribuir o novo valor de volta ao pixel
                            dataPtr[channel] = (byte)pixelValue;
                        }

                        // Avançar para o próximo pixel
                        dataPtr += nChan;
                    }

                    // Avançar para a próxima linha (considerar o padding)
                    dataPtr += padding;
                }
            }
        }


        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // store in the image
                            dataPtr[0] = red;
                            dataPtr[1] = red;
                            dataPtr[2] = red;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // store in the image
                            dataPtr[0] = (byte)(255 - blue);
                            dataPtr[1] = (byte)(255 - green);
                            dataPtr[2] = (byte)(255 - red);

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrimgCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                int padding = widthStep - nChan * width;

                int last_h = height - 1;
                int last_w = width - 1;

                // Canto superior esquerdo (top-left corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy + nChan)[0] +
                                    2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + nChan + widthStep)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy + nChan)[1] +
                                    2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + nChan + widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy + nChan)[2] +
                                    2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + nChan + widthStep)[2]) / 9.0);

                dataPtrImg += nChan;
                dataPtrimgCopy += nChan;

                // Linha de cima (top row)
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[0] + 2 * dataPtrimgCopy[0] +
                                        2 * (dataPtrimgCopy + nChan)[0] + (dataPtrimgCopy - nChan + widthStep)[0] +
                                        (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + nChan + widthStep)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[1] + 2 * dataPtrimgCopy[1] +
                                        2 * (dataPtrimgCopy + nChan)[1] + (dataPtrimgCopy - nChan + widthStep)[1] +
                                        (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + nChan + widthStep)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[2] + 2 * dataPtrimgCopy[2] +
                                        2 * (dataPtrimgCopy + nChan)[2] + (dataPtrimgCopy - nChan + widthStep)[2] +
                                        (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + nChan + widthStep)[2]) / 9.0);

                    dataPtrImg += nChan;
                    dataPtrimgCopy += nChan;
                }

                // Canto superior direito (top-right corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy - nChan)[0] +
                                    2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy - nChan + widthStep)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy - nChan)[1] +
                                    2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy - nChan + widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy - nChan)[2] +
                                    2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy - nChan + widthStep)[2]) / 9.0);

                dataPtrImg += nChan + padding;
                dataPtrimgCopy += nChan + padding;

                // Process interior and sides of the image
                for (int y = 1; y < last_h; y++)
                {
                    // Margem esquerda (left margin)
                    dataPtrImg[0] = (byte)Math.Round((2 * dataPtrimgCopy[0] + (dataPtrimgCopy + nChan)[0] +
                                        2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + nChan + widthStep)[0] +
                                        2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * dataPtrimgCopy[1] + (dataPtrimgCopy + nChan)[1] +
                                        2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + nChan + widthStep)[1] +
                                        2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * dataPtrimgCopy[2] + (dataPtrimgCopy + nChan)[2] +
                                        2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + nChan + widthStep)[2] +
                                        2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2]) / 9.0);

                    dataPtrImg += nChan;
                    dataPtrimgCopy += nChan;

                    // Process internal pixels
                    for (int x = 1; x < last_w; x++)
                    {
                        dataPtrImg[0] = (byte)Math.Round((dataPtrimgCopy[0] + (dataPtrimgCopy + nChan)[0] +
                                            (dataPtrimgCopy - nChan)[0] + (dataPtrimgCopy + widthStep + nChan)[0] +
                                            (dataPtrimgCopy + widthStep - nChan)[0] + (dataPtrimgCopy + widthStep)[0] +
                                            (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0] +
                                            (dataPtrimgCopy - widthStep - nChan)[0]) / 9.0);
                        dataPtrImg[1] = (byte)Math.Round((dataPtrimgCopy[1] + (dataPtrimgCopy + nChan)[1] +
                                            (dataPtrimgCopy - nChan)[1] + (dataPtrimgCopy + widthStep + nChan)[1] +
                                            (dataPtrimgCopy + widthStep - nChan)[1] + (dataPtrimgCopy + widthStep)[1] +
                                            (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1] +
                                            (dataPtrimgCopy - widthStep - nChan)[1]) / 9.0);
                        dataPtrImg[2] = (byte)Math.Round((dataPtrimgCopy[2] + (dataPtrimgCopy + nChan)[2] +
                                            (dataPtrimgCopy - nChan)[2] + (dataPtrimgCopy + widthStep + nChan)[2] +
                                            (dataPtrimgCopy + widthStep - nChan)[2] + (dataPtrimgCopy + widthStep)[2] +
                                            (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2] +
                                            (dataPtrimgCopy - widthStep - nChan)[2]) / 9.0);

                        dataPtrImg += nChan;
                        dataPtrimgCopy += nChan;
                    }

                    // Margem direita (right margin)
                    dataPtrImg[0] = (byte)Math.Round((2 * dataPtrimgCopy[0] + (dataPtrimgCopy - nChan)[0] +
                                        2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + widthStep - nChan)[0] +
                                        2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep - nChan)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * dataPtrimgCopy[1] + (dataPtrimgCopy - nChan)[1] +
                                        2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + widthStep - nChan)[1] +
                                        2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep - nChan)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * dataPtrimgCopy[2] + (dataPtrimgCopy - nChan)[2] +
                                        2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + widthStep - nChan)[2] +
                                        2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep - nChan)[2]) / 9.0);

                    dataPtrImg += nChan + padding;
                    dataPtrimgCopy += nChan + padding;
                }

                // Canto inferior esquerdo (bottom-left corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy + nChan)[0] +
                                    2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy + nChan)[1] +
                                    2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy + nChan)[2] +
                                    2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2]) / 9.0);

                dataPtrImg += nChan;
                dataPtrimgCopy += nChan;

                // Linha de baixo (bottom row)
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[0] + 2 * dataPtrimgCopy[0] +
                                        2 * (dataPtrimgCopy + nChan)[0] + (dataPtrimgCopy - widthStep - nChan)[0] +
                                        (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[1] + 2 * dataPtrimgCopy[1] +
                                        2 * (dataPtrimgCopy + nChan)[1] + (dataPtrimgCopy - widthStep - nChan)[1] +
                                        (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[2] + 2 * dataPtrimgCopy[2] +
                                        2 * (dataPtrimgCopy + nChan)[2] + (dataPtrimgCopy - widthStep - nChan)[2] +
                                        (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2]) / 9.0);

                    dataPtrImg += nChan;
                    dataPtrimgCopy += nChan;
                }

                // Canto inferior direito (bottom-right corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy - nChan)[0] +
                                    2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep - nChan)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy - nChan)[1] +
                                    2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep - nChan)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy - nChan)[2] +
                                    2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep - nChan)[2]) / 9.0);
            }
        }
        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();           // pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();   // pointer to the image copy

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels (3 for RGB)
                int padding = m.widthStep - nChan * width;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        // Média para o canal azul
                        int blueSum = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[0] * matrix[0, 0]) +
                                     ((dataPtrCopy + nChan * x + mCopy.widthStep * (y - 1))[0] * matrix[0, 1]) +
                                     ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[0] * matrix[0, 2]) +
                                     ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * y)[0] * matrix[1, 0]) +
                                     ((dataPtrCopy + nChan * x + mCopy.widthStep * y)[0] * matrix[1, 1]) +
                                     ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * y)[0] * matrix[1, 2]) +
                                     ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[0] * matrix[2, 0]) +
                                     ((dataPtrCopy + nChan * x + mCopy.widthStep * (y + 1))[0] * matrix[2, 1]) +
                                     ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[0]) * matrix[2, 2]);


                        int greenSum = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[1] * matrix[0, 0]) +
                                      ((dataPtrCopy + nChan * x + mCopy.widthStep * (y - 1))[1] * matrix[0, 1]) +
                                      ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[1] * matrix[0, 2]) +
                                      ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * y)[1] * matrix[1, 0]) +
                                      ((dataPtrCopy + nChan * x + mCopy.widthStep * y)[1] * matrix[1, 1]) +
                                      ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * y)[1] * matrix[1, 2]) +
                                      ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[1] * matrix[2, 0]) +
                                      ((dataPtrCopy + nChan * x + mCopy.widthStep * (y + 1))[1] * matrix[2, 1]) +
                                      ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[1]) * matrix[2, 2]);

                        int redSum = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[2] * matrix[0, 0]) +
                                      ((dataPtrCopy + nChan * x + mCopy.widthStep * (y - 1))[2] * matrix[0, 1]) +
                                      ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[2] * matrix[0, 2]) +
                                      ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * y)[2] * matrix[1, 0]) +
                                      ((dataPtrCopy + nChan * x + mCopy.widthStep * y)[2] * matrix[1, 1]) +
                                      ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * y)[2] * matrix[1, 2]) +
                                      ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[2] * matrix[2, 0]) +
                                      ((dataPtrCopy + nChan * x + mCopy.widthStep * (y + 1))[2] * matrix[2, 1]) +
                                      ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[2]) * matrix[2, 2]);

                        if ((int)Math.Round(blueSum / matrixWeight) > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 255;
                        }
                        else if ((int)Math.Round(blueSum / matrixWeight) <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = (byte)Math.Round(blueSum / matrixWeight);
                        }

                        if ((int)Math.Round(greenSum / matrixWeight) > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 255;
                        }
                        else if ((int)Math.Round(greenSum / matrixWeight) <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = (byte)Math.Round(greenSum / matrixWeight);
                        }

                        if ((int)Math.Round(redSum / matrixWeight) > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 255;
                        }
                        else if ((int)Math.Round(redSum / matrixWeight) <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = (byte)Math.Round(redSum / matrixWeight);
                        }

                    }
                }

                for (int x = 1; x < width - 1; x++)
                {
                    // Primeira linha
                    int sumBFirstLine = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[0] * (matrix[1, 0] + matrix[0, 0])) +  // (x-1, 0)
                                             ((dataPtrCopy + nChan * x + mCopy.widthStep * 0)[0] * (matrix[1, 1] + matrix[0, 1])) +        // (x, 0)
                                             ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[0] * (matrix[1, 2] + matrix[0, 2])) +  // (x+1, 0)
                                             ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[0] * (matrix[2, 0])) +        // (x-1, 1)
                                             ((dataPtrCopy + nChan * x + mCopy.widthStep * 1)[0] * (matrix[2, 1])) +              // (x, 1)
                                             ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[0] * (matrix[2, 2])));         // (x+1, 1)); 

                    if ((int)Math.Round(sumBFirstLine / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[0] = 255;
                    }
                    else if ((int)Math.Round(sumBFirstLine / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[0] = (byte)Math.Round(sumBFirstLine / matrixWeight);
                    }// Blue


                    int sumGFirstLine = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[1] * (matrix[1, 0] + matrix[0, 0])) +  // (x-1, 0)
                        ((dataPtrCopy + nChan * x + mCopy.widthStep * 0)[1] * (matrix[1, 1] + matrix[0, 1])) +      // (x, 0)
                        ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[1] * (matrix[1, 2] + matrix[0, 2])) + // (x+1, 0)
                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[1] * (matrix[2, 0])) + // (x-1, 1)
                        ((dataPtrCopy + nChan * x + mCopy.widthStep * 1)[1] * (matrix[2, 1])) +       // (x, 1)
                        ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[1] * (matrix[2, 2]))); // (x+1, 1)

                    if ((int)Math.Round(sumGFirstLine / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[1] = 255;
                    }
                    else if ((int)Math.Round(sumGFirstLine / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[1] = (byte)Math.Round(sumGFirstLine / matrixWeight);
                    } // Green

                    int sumRFirstLine = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[2] * (matrix[1, 0] + matrix[0, 0])) +  // (x-1, 0)
                        ((dataPtrCopy + nChan * x + mCopy.widthStep * 0)[2] * (matrix[1, 1] + matrix[0, 1])) +      // (x, 0)
                        ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[2] * (matrix[1, 2] + matrix[0, 2])) + // (x+1, 0)
                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[2] * (matrix[2, 0])) + // (x-1, 1)
                        ((dataPtrCopy + nChan * x + mCopy.widthStep * 1)[2] * (matrix[2, 1])) +       // (x, 1)
                        ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[2] * (matrix[2, 2]))); // (x+1, 1)

                    if ((int)Math.Round(sumRFirstLine / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[2] = 255;
                    }
                    else if ((int)Math.Round(sumRFirstLine / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[2] = (byte)Math.Round(sumRFirstLine / matrixWeight);
                    } // Red

                    // Última linha
                    int sumBLastLine = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[0] * matrix[0, 0]) +  // (x-1, height-2)
                       ((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 2))[0] * matrix[0, 1]) +       // (x, height-2)
                       ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[0] * matrix[0, 2]) + // (x+1, height-2)
                       ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[0] * (matrix[1, 0] + matrix[2, 0])) + // (x-1, height-1)
                       ((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0] * (matrix[1, 1] + matrix[2, 1])) +       // (x, height-1)
                       ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0] * (matrix[1, 2] + matrix[2, 2]))); // (x+1, height-1) 

                    if ((int)Math.Round(sumBLastLine / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if ((int)Math.Round(sumBLastLine / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = (byte)Math.Round(sumBLastLine / matrixWeight);
                    } // Blue

                    int sumGLastLine = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[1] * matrix[0, 0]) +  // (x-1, height-2)
                       ((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 2))[1] * matrix[0, 1]) +       // (x, height-2)
                       ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[1] * matrix[0, 2]) + // (x+1, height-2)
                       ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[1] * (matrix[1, 0] + matrix[2, 0])) + // (x-1, height-1)
                       ((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1] * (matrix[1, 1] + matrix[2, 1])) +       // (x, height-1)
                       ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1] * (matrix[1, 2] + matrix[2, 2]))); // (x+1, height-1) 

                    if ((int)Math.Round(sumGLastLine / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if ((int)Math.Round(sumGLastLine / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = (byte)Math.Round(sumGLastLine / matrixWeight);
                    } // Green

                    int sumRLastLine = (int)(((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[2] * matrix[0, 0]) +  // (x-1, height-2)
                        ((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 2))[2] * matrix[0, 1]) +       // (x, height-2)
                        ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[2] * matrix[0, 2]) + // (x+1, height-2)
                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[2] * (matrix[1, 0] + matrix[2, 0])) + // (x-1, height-1)
                        ((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2] * (matrix[1, 1] + matrix[2, 1])) +       // (x, height-1)
                        ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2] * (matrix[1, 2] + matrix[2, 2]))); // (x+1, height-1) 

                    if ((int)Math.Round(sumRLastLine / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if ((int)Math.Round(sumRLastLine / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = (byte)Math.Round(sumRLastLine / matrixWeight);
                    } // Red
                }


                // colunas
                for (int y = 1; y < height - 1; y++)
                {
                    // Primeira coluna
                    int sumBColEsq = (int)(
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[0] * (matrix[0, 1] + matrix[0, 0])) +  // (0, y-1)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * y)[0] * (matrix[1, 1] + matrix[1, 0])) +      // (0, y)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[0] * (matrix[2, 1] + matrix[2, 0])) + // (0, y+1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[0] * (matrix[0, 2])) +    // (1, y-1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * y)[0] * matrix[1, 2]) +          // (1, y)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[0] * matrix[2, 2]));    // (1, y+1)

                    if ((int)Math.Round(sumBColEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[0] = 255;
                    }
                    else if ((int)Math.Round(sumBColEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[0] = (byte)Math.Round(sumBColEsq / matrixWeight);
                    }
                    // Blue

                    int sumGColEsq = (int)(
                       ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[1] * (matrix[0, 1] + matrix[0, 0])) +  // (0, y-1)
                       ((dataPtrCopy + nChan * 0 + mCopy.widthStep * y)[1] * (matrix[1, 1] + matrix[1, 0])) +      // (0, y)
                       ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[1] * (matrix[2, 1] + matrix[2, 0])) + // (0, y+1)
                       ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[1] * (matrix[0, 2])) +    // (1, y-1)
                       ((dataPtrCopy + nChan * 1 + mCopy.widthStep * y)[1] * matrix[1, 2]) +          // (1, y)
                       ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[1] * matrix[2, 2]));    // (1, y+1)

                    if ((int)Math.Round(sumGColEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[1] = 255;
                    }
                    else if ((int)Math.Round(sumGColEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[1] = (byte)Math.Round(sumGColEsq / matrixWeight);
                    }
                    // Green

                    int sumRColEsq = (int)(
                       ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[2] * (matrix[0, 1] + matrix[0, 0])) +  // (0, y-1)
                       ((dataPtrCopy + nChan * 0 + mCopy.widthStep * y)[2] * (matrix[1, 1] + matrix[1, 0])) +      // (0, y)
                       ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[2] * (matrix[2, 1] + matrix[2, 0])) + // (0, y+1)
                       ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[2] * (matrix[0, 2])) +    // (1, y-1)
                       ((dataPtrCopy + nChan * 1 + mCopy.widthStep * y)[2] * matrix[1, 2]) +          // (1, y)
                       ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[2] * matrix[2, 2]));    // (1, y+1)

                    if ((int)Math.Round(sumRColEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[2] = 255;
                    }
                    else if ((int)Math.Round(sumRColEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[2] = (byte)Math.Round(sumRColEsq / matrixWeight);
                    } // Red

                    // Última coluna
                    int sumBColDir = (int)(
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[0] * matrix[0, 0]) +  // (width-2, y-1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[0] * (matrix[0, 1] + matrix[0, 2])) +  // (width-1, y-1)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * y)[0] * matrix[1, 0]) +      // (width-2, y)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0] * (matrix[1, 1] + matrix[1, 2])) +      // (width-1, y)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[0] * matrix[2, 0]) + // (width-2, y+1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0] * (matrix[2, 1] + matrix[2, 2]))); // (width-1, y+1)

                    if ((int)Math.Round(sumBColDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 255;
                    }
                    else if ((int)Math.Round(sumBColDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = (byte)Math.Round(sumBColDir / matrixWeight);
                    }
                    // Blue

                    int sumGColDir = (int)(
                    ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[1] * matrix[0, 0]) +  // (width-2, y-1)
                    ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[1] * (matrix[0, 1] + matrix[0, 2])) +  // (width-1, y-1)
                    ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * y)[1] * matrix[1, 0]) +      // (width-2, y)
                    ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1] * (matrix[1, 1] + matrix[1, 2])) +      // (width-1, y)
                    ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[1] * matrix[2, 0]) + // (width-2, y+1)
                    ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1] * (matrix[2, 1] + matrix[2, 2]))); // (width-1, y+1)

                    if ((int)Math.Round(sumGColDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 255;
                    }
                    else if ((int)Math.Round(sumGColDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = (byte)Math.Round(sumGColDir / matrixWeight);
                    }
                    // Green

                    int sumRColDir = (int)(
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[2] * matrix[0, 0]) +  // (width-2, y-1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[2] * (matrix[0, 1] + matrix[0, 2])) +  // (width-1, y-1)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * y)[2] * matrix[1, 0]) +      // (width-2, y)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2] * (matrix[1, 1] + matrix[1, 2])) +      // (width-1, y)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[2] * matrix[2, 0]) + // (width-2, y+1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2] * (matrix[2, 1] + matrix[2, 2]))); // (width-1, y+1)

                    if ((int)Math.Round(sumRColDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 255;
                    }
                    else if ((int)Math.Round(sumRColDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = (byte)Math.Round(sumRColDir / matrixWeight);
                    } // Red
                }


                // cantos
                {
                    int sumBCantoEsq = (int)(
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] * (matrix[0, 0] + matrix[1, 0] + matrix[0, 1] + matrix[1, 1])) +  // (0, 0)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[0] * (matrix[1, 2] + matrix[0, 2])) +  // (1, 0)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[0] * (matrix[2, 0] + matrix[2, 1])) +  // (0, 1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[0] * matrix[2, 2]));  // (1, 1)


                    if ((int)Math.Round(sumBCantoEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 255;
                    }
                    else if ((int)Math.Round(sumBCantoEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = (byte)Math.Round(sumBCantoEsq / matrixWeight);
                    } // Blue

                    int sumGCantoEsq = (int)(
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] * (matrix[0, 0] + matrix[1, 0] + matrix[0, 1] + matrix[1, 1])) +  // (0, 0)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[1] * (matrix[1, 2] + matrix[0, 2])) +  // (1, 0)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[1] * (matrix[2, 0] + matrix[2, 1])) +  // (0, 1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[1] * matrix[2, 2]));  // (1, 1)


                    if ((int)Math.Round(sumGCantoEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 255;
                    }
                    else if ((int)Math.Round(sumGCantoEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = (byte)Math.Round(sumGCantoEsq / matrixWeight);
                    } // Green

                    int sumRCantoEsq = (int)(
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] * (matrix[0, 0] + matrix[1, 0] + matrix[0, 1] + matrix[1, 1])) +  // (0, 0)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[2] * (matrix[1, 2] + matrix[0, 2])) +  // (1, 0)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[2] * (matrix[2, 0] + matrix[2, 1])) +  // (0, 1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[2] * matrix[2, 2]));  // (1, 1)


                    if ((int)Math.Round(sumRCantoEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 255;
                    }
                    else if ((int)Math.Round(sumRCantoEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = (byte)Math.Round(sumRCantoEsq / matrixWeight);
                    }  // Red
                }

                // canto sup dire
                {
                    int sumBlueCantoSupDir = (int)(((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] * (matrix[0, 1] + matrix[1, 1] + matrix[0, 2] + matrix[1, 2])) +  // (width-1, 0)
                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[0] * (matrix[0, 0] + matrix[1, 0])) +  // (width-2, 0)
                        ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0] * (matrix[2, 1] + matrix[2, 2])) +  // (width-1, 1)
                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[0] * matrix[2, 0]));   // (width-2, 1)


                    if ((int)Math.Round(sumBlueCantoSupDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 255;
                    }
                    else if ((int)Math.Round(sumBlueCantoSupDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = (byte)Math.Round(sumBlueCantoSupDir / matrixWeight);
                    } // Blue

                    int sumGreenCantoSupDir = (int)(((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] * (matrix[0, 1] + matrix[1, 1] + matrix[0, 2] + matrix[1, 2])) +  // (width-1, 0)
                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[1] * (matrix[0, 0] + matrix[1, 0])) +  // (width-2, 0)
                        ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1] * (matrix[2, 1] + matrix[2, 2])) +  // (width-1, 1)
                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[1] * matrix[2, 0]));   // (width-2, 1)


                    if ((int)Math.Round(sumGreenCantoSupDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 255;
                    }
                    else if ((int)Math.Round(sumGreenCantoSupDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = (byte)Math.Round(sumGreenCantoSupDir / matrixWeight);
                    } // Green

                    int sumRedCantoSupDir = (int)(((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] * (matrix[0, 1] + matrix[1, 1] + matrix[0, 2] + matrix[1, 2])) +  // (width-1, 0)
                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[2] * (matrix[0, 0] + matrix[1, 0])) +  // (width-2, 0)
                        ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2] * (matrix[2, 1] + matrix[2, 2])) +  // (width-1, 1)
                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[2] * matrix[2, 0]));   // (width-2, 1)


                    if ((int)Math.Round(sumRedCantoSupDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 255;
                    }
                    else if ((int)Math.Round(sumRedCantoSupDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = (byte)Math.Round(sumRedCantoSupDir / matrixWeight);
                    }; // Red
                }

                // canto inf esq
                {
                    int sumBlueCantoInfEsq = (int)(((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] * (matrix[1, 0] + matrix[2, 0] + matrix[1, 1] + matrix[2, 1])) +  // (0, height-1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0] * (matrix[1, 2] + matrix[2, 2])) +  // (1, height-1)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[0] * (matrix[0, 0] + matrix[0, 1])) +  // (0, height-2)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[0] * (matrix[0, 2])));  // (1, height-2)

                    if ((int)Math.Round(sumBlueCantoInfEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if ((int)Math.Round(sumBlueCantoInfEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = (byte)Math.Round(sumBlueCantoInfEsq / matrixWeight);
                    }; // Blue

                    int sumGreenCantoInfEsq = (int)(((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] * (matrix[1, 0] + matrix[2, 0] + matrix[1, 1] + matrix[2, 1])) +  // (0, height-1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1] * (matrix[1, 2] + matrix[2, 2])) +  // (1, height-1)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[1] * (matrix[0, 0] + matrix[0, 1])) +  // (0, height-2)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[1] * (matrix[0, 2])));  // (1, height-2)

                    if ((int)Math.Round(sumGreenCantoInfEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if ((int)Math.Round(sumGreenCantoInfEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = (byte)Math.Round(sumGreenCantoInfEsq / matrixWeight);
                    }; // Green

                    int sumRedCantoInfEsq = (int)(((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] * (matrix[1, 0] + matrix[2, 0] + matrix[1, 1] + matrix[2, 1])) +  // (0, height-1)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2] * (matrix[1, 2] + matrix[2, 2])) +  // (1, height-1)
                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[2] * (matrix[0, 0] + matrix[0, 1])) +  // (0, height-2)
                        ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[2] * (matrix[0, 2])));  // (1, height-2)

                    if ((int)Math.Round(sumRedCantoInfEsq / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if ((int)Math.Round(sumRedCantoInfEsq / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = (byte)Math.Round(sumRedCantoInfEsq / matrixWeight);
                    }; // Red
                }

                // canto inf dire
                {
                    int sumBlueCantoInfDir = (int)(
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[0] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])) +  // (width-1, height-1)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[0] * (matrix[1, 0] + matrix[2, 0])) +  // (width-2, height-1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[0] * (matrix[0, 1] + matrix[0, 2])) +  // (width-1, height-2)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[0] * (matrix[0, 0])));  // (width-2, height-2)


                    if ((int)Math.Round(sumBlueCantoInfDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if ((int)Math.Round(sumBlueCantoInfDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = (byte)Math.Round(sumBlueCantoInfDir / matrixWeight);
                    }
                    // Blue

                    int sumGreenCantoInfDir = (int)(
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[1] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])) +  // (width-1, height-1)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[1] * (matrix[1, 0] + matrix[2, 0])) +  // (width-2, height-1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[1] * (matrix[0, 1] + matrix[0, 2])) +  // (width-1, height-2)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[1] * (matrix[0, 0])));  // (width-2, height-2)


                    if ((int)Math.Round(sumGreenCantoInfDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if ((int)Math.Round(sumGreenCantoInfDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = (byte)Math.Round(sumGreenCantoInfDir / matrixWeight);
                    }; // Green

                    int sumRedCantoInfDir = (int)(
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[2] * (matrix[1, 1] + matrix[1, 2] + matrix[2, 1] + matrix[2, 2])) +  // (width-1, height-1)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[2] * (matrix[1, 0] + matrix[2, 0])) +  // (width-2, height-1)
                   ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[2] * (matrix[0, 1] + matrix[0, 2])) +  // (width-1, height-2)
                   ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[2] * (matrix[0, 0])));  // (width-2, height-2)


                    if ((int)Math.Round(sumRedCantoInfDir / matrixWeight) > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if ((int)Math.Round(sumRedCantoInfDir / matrixWeight) <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = (byte)Math.Round(sumRedCantoInfDir / matrixWeight);
                    }  // Red
                }

            }
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();           // pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();   // pointer to the image copy

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels (3 for RGB)
                int padding = m.widthStep - nChan * width;

                // Sobel filter for each pixel
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        // Gradient in the X direction (Sobel X)
                        int blueSumX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[0] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * y)[0] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[0]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[0] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * y)[0] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[0]);

                        int blueSumY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[0] +
                                            2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (y + 1))[0] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[0]) -
                                            ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[0] +
                                            2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (y - 1))[0] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[0]);

                        int sumBlueAbs = Math.Abs(blueSumX) + Math.Abs(blueSumY);

                        // Clamp to [0, 255]
                        if (sumBlueAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 255;
                        }
                        else if (sumBlueAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = (byte)sumBlueAbs;
                        }


                        // Gradient for the green channel
                        int greenSumX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[1] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * y)[1] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[1]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[1] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * y)[1] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[1]);

                        int greenSumY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[1] +
                                            2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (y + 1))[1] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[1]) -
                                            ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[1] +
                                            2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (y - 1))[1] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[1]);

                        int sumGreenAbs = Math.Abs(greenSumX) + Math.Abs(greenSumY);

                        // Clamp to [0, 255]
                        if (sumGreenAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 255;
                        }
                        else if (sumGreenAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = (byte)sumGreenAbs;
                        }


                        // Gradient for the red channel
                        int redSumX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[2] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * y)[2] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[2]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[2] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * y)[2] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[2]);

                        int redSumY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y + 1))[2] +
                                            2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (y + 1))[2] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y + 1))[2]) -
                                            ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (y - 1))[2] +
                                            2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (y - 1))[2] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (y - 1))[2]);

                        int sumRedAbs = Math.Abs(redSumX) + Math.Abs(redSumY);

                        // Clamp to [0, 255]
                        if (sumRedAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 255;
                        }
                        else if (sumRedAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = (byte)sumRedAbs;
                        }

                    }
                }
                //linhas
                for (int x = 1; x < width - 1; x++)
                {
                    //linha cima
                    int blueSumFirstLineX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[0] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[0] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[0]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[0] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[0] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[0]);

                    int blueSumYFirstLineY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * 1)[0] + // h (x,1)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * 0)[0] + //b (x,0)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[0]); //c (x+1,0)

                    int sumBlueAbsFirstLine = Math.Abs(blueSumFirstLineX) + Math.Abs(blueSumYFirstLineY);

                    // Clamp to [0, 255]
                    if (sumBlueAbsFirstLine > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueAbsFirstLine <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[0] = (byte)sumBlueAbsFirstLine;
                    }

                    int greenSumFirstLineX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[1] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[1] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[1]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[1] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[1] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[1]);

                    int greenSumYFirstLineY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * 1)[1] + // h (x,1)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * 0)[1] + //b (x,0)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[1]); //c (x+1,0)

                    int sumGreenAbsFirstLine = Math.Abs(greenSumFirstLineX) + Math.Abs(greenSumYFirstLineY);

                    // Clamp to [0, 255]
                    if (sumGreenAbsFirstLine > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenAbsFirstLine <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[1] = (byte)sumGreenAbsFirstLine;
                    }

                    int redSumFirstLineX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[2] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[2] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[2]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[2] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[2] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[2]);

                    int redSumYFirstLineY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 1)[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * 1)[2] + // h (x,1)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 1)[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * 0)[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * 0)[2] + //b (x,0)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * 0)[2]); //c (x+1,0)

                    int sumRedAbsFirstLine = Math.Abs(redSumFirstLineX) + Math.Abs(redSumYFirstLineY);

                    // Clamp to [0, 255]
                    if (sumRedAbsFirstLine > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedAbsFirstLine <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * 0)[2] = (byte)sumRedAbsFirstLine;
                    }

                    //last line

                    int blueSumLastLineX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[0] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[0] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[0]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[0] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0]);

                    int blueSumLastLineY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0] + // h (x,1)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 2))[0] + //b (x,0)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[0]); //c (x+1,0)

                    int sumBlueAbsLastLine = Math.Abs(blueSumLastLineX) + Math.Abs(blueSumLastLineY);

                    // Clamp to [0, 255]
                    if (sumBlueAbsLastLine > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if (sumBlueAbsLastLine <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = (byte)sumBlueAbsLastLine;
                    }

                    int greenSumLastLineX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[1] +
                                            2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[1] +
                                            (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[1]) -
                                            ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[1] +
                                            2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1] +
                                            (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1]);

                    int greenSumLastLineY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1] + // h (x,1)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 2))[1] + //b (x,0)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[1]); //c (x+1,0)

                    int sumGreenAbsLastLine = Math.Abs(greenSumLastLineX) + Math.Abs(greenSumLastLineY);

                    // Clamp to [0, 255]
                    if (sumGreenAbsLastLine > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if (sumGreenAbsLastLine <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = (byte)sumGreenAbsLastLine;
                    }

                    int redSumLastLineX = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[2] +
                                             2 * (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[2] +
                                             (dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[2]) -
                                             ((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[2] +
                                             2 * (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2] +
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2]);

                    int redSumLastLineY = (int)((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 1))[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2] + // h (x,1)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (x - 1) + mCopy.widthStep * (height - 2))[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 2))[2] + //b (x,0)
                                        (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 2))[2]); //c (x+1,0)

                    int sumRedAbsLastLine = Math.Abs(redSumLastLineX) + Math.Abs(redSumLastLineY);

                    // Clamp to [0, 255]
                    if (sumRedAbsLastLine > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if (sumRedAbsLastLine <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = (byte)sumRedAbsLastLine;
                    }
                }
                //colunas
                for (int y = 1; y < height - 1; y++)
                {
                    //primeira coluna
                    int blueSumFirstColX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[0] +
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * y)[0] +
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[0]) -
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[0] +
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * y)[0] +
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[0]);

                    int blueSumFirstColY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[0] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[0] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[0]); //c (x+1,0)

                    int sumBlueAbsFirstCol = Math.Abs(blueSumFirstColX) + Math.Abs(blueSumFirstColY);

                    // Clamp to [0, 255]
                    if (sumBlueAbsFirstCol > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[0] = 255;
                    }
                    else if (sumBlueAbsFirstCol <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[0] = (byte)sumBlueAbsFirstCol;
                    }
                    int greenSumFirstColX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[1] +
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * y)[1] +
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[1]) -
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[1] +
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * y)[1] +
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[1]);

                    int greenSumFirstColY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[1] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[1] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[1]); //c (x+1,0)

                    int sumGreenAbsFirstCol = Math.Abs(greenSumFirstColX) + Math.Abs(greenSumFirstColY);

                    // Clamp to [0, 255]
                    if (sumGreenAbsFirstCol > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[1] = 255;
                    }
                    else if (sumGreenAbsFirstCol <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[1] = (byte)sumGreenAbsFirstCol;
                    }

                    int redSumFirstColX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[2] +
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * y)[2] +
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[2]) -
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[2] +
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * y)[2] +
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[2]);

                    int redSumFirstColY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y + 1))[2] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y + 1))[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (y - 1))[2] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (y - 1))[2]); //c (x+1,0)

                    int sumRedAbsFirstCol = Math.Abs(redSumFirstColX) + Math.Abs(redSumFirstColY);

                    // Clamp to [0, 255]
                    if (sumRedAbsFirstCol > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[2] = 255;
                    }
                    else if (sumRedAbsFirstCol <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * y)[2] = (byte)sumRedAbsFirstCol;
                    }

                    //ultima coluna
                    int blueSumLastColX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[0] +
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * y)[0] +
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[0]) -
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[0] +
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0] +
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0]);

                    int blueSumLastColY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[0] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[0]); //c (x+1,0)

                    int sumBlueAbsLastCol = Math.Abs(blueSumLastColX) + Math.Abs(blueSumLastColY);

                    // Clamp to [0, 255]
                    if (sumBlueAbsLastCol > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 255;
                    }
                    else if (sumBlueAbsLastCol <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = (byte)sumBlueAbsLastCol;
                    }

                    int greenSumLastColX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[1] +
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * y)[1] +
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[1]) -
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[1] +
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1] +
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1]);

                    int greenSumLastColY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[1] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[1]); //c (x+1,0)

                    int sumGreenAbsLastCol = Math.Abs(greenSumLastColX) + Math.Abs(greenSumLastColY);

                    // Clamp to [0, 255]
                    if (sumGreenAbsLastCol > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 255;
                    }
                    else if (sumGreenAbsLastCol <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = (byte)sumGreenAbsLastCol;
                    }

                    int redSumLastColX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[2] +
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * y)[2] +
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[2]) -
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[2] +
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2] +
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2]);

                    int redSumLastColY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y + 1))[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (y - 1))[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[2] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y - 1))[2]); //c (x+1,0)

                    int sumRedAbsLastCol = Math.Abs(redSumLastColX) + Math.Abs(redSumLastColY);

                    // Clamp to [0, 255]
                    if (sumRedAbsLastCol > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 255;
                    }
                    else if (sumRedAbsLastCol <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = (byte)sumRedAbsLastCol;
                    }

                }
                {
                    //canto superior esquerdo
                    int blueSumCantoSupEsqX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] +
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] +
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[0]) -
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[0] +
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[0] +
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[0]);

                    int blueSumCantoSupEsqY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[0] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[0]); //c (x+1,0)

                    int sumBlueAbsCantoSupEsq = Math.Abs(blueSumCantoSupEsqX) + Math.Abs(blueSumCantoSupEsqY);

                    if (sumBlueAbsCantoSupEsq > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueAbsCantoSupEsq <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = (byte)sumBlueAbsCantoSupEsq;
                    }

                    int greenSumCantoSupEsqX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] +
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] +
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[1]) -
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[1] +
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[1] +
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[1]);

                    int greenSumCantoSupEsqY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[1] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[1]); //c (x+1,0)

                    int sumGreenAbsCantoSupEsq = Math.Abs(greenSumCantoSupEsqX) + Math.Abs(greenSumCantoSupEsqY);

                    if (sumGreenAbsCantoSupEsq > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenAbsCantoSupEsq <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = (byte)sumGreenAbsCantoSupEsq;
                    }

                    int redSumCantoSupEsqX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] +
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] +
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[2]) -
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[2] +
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[2] +
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[2]);

                    int redSumCantoSupEsqY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[2] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[2]); //c (x+1,0)

                    int sumRedAbsCantoSupEsq = Math.Abs(redSumCantoSupEsqX) + Math.Abs(redSumCantoSupEsqY);

                    if (sumRedAbsCantoSupEsq > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedAbsCantoSupEsq <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = (byte)sumRedAbsCantoSupEsq;
                    }

                }
                {
                    // canto superior direito

                    int blueSumCantoSupDirX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[0] +
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[0] +
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[0]) -
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] +
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] +
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0]);

                    int blueSumCantoSupDirY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0]); //c (x+1,0)

                    int sumBlueAbsCantoSupEsq = Math.Abs(blueSumCantoSupDirX) + Math.Abs(blueSumCantoSupDirY);

                    if (sumBlueAbsCantoSupEsq > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueAbsCantoSupEsq <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = (byte)sumBlueAbsCantoSupEsq;
                    }

                    int greenSumCantoSupDirX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[1] +
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[1] +
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[1]) -
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] +
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] +
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1]);

                    int greenSumCantoSupDirY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1]); //c (x+1,0)

                    int sumGreenAbsCantoSupEsq = Math.Abs(greenSumCantoSupDirX) + Math.Abs(greenSumCantoSupDirY);

                    if (sumGreenAbsCantoSupEsq > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenAbsCantoSupEsq <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = (byte)sumGreenAbsCantoSupEsq;
                    }

                    int redSumCantoSupDirX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[2] +
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[2] +
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[2]) -
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] +
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] +
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2]);

                    int redSumCantoSupDirY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 1)[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * 0)[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2]); //c (x+1,0)

                    int sumRedAbsCantoSupEsq = Math.Abs(redSumCantoSupDirX) + Math.Abs(redSumCantoSupDirY);

                    if (sumRedAbsCantoSupEsq > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedAbsCantoSupEsq <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = (byte)sumRedAbsCantoSupEsq;
                    }
                }
                {
                    //canto inferior esquerdo

                    int blueSumCantoInfEsqX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[0] + //a
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] + //d
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0]) - //g
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[0] + //c
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0] + //f
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0]); //i

                    int blueSumCantoInfEsqY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[0] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[0]); //c (x+1,0)

                    int sumBlueAbsCantoInfEsq = Math.Abs(blueSumCantoInfEsqX) + Math.Abs(blueSumCantoInfEsqY);

                    if (sumBlueAbsCantoInfEsq > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if (sumBlueAbsCantoInfEsq <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = (byte)sumBlueAbsCantoInfEsq;
                    }

                    int greenSumCantoInfEsqX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[1] + //a
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] + //d
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1]) - //g
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[1] + //c
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1] + //f
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1]); //i

                    int greenSumCantoInfEsqY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[1] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[1]); //c (x+1,0)

                    int sumGreenAbsCantoInfEsq = Math.Abs(greenSumCantoInfEsqX) + Math.Abs(greenSumCantoInfEsqY);

                    if (sumGreenAbsCantoInfEsq > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if (sumGreenAbsCantoInfEsq <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = (byte)sumGreenAbsCantoInfEsq;
                    }

                    int redSumCantoInfEsqX = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[2] + //a
                                                2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] + //d
                                                (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2]) - //g
                                                ((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[2] + //c
                                                2 * (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2] + //f
                                                (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2]); //i

                    int redSumCantoInfEsqY = (int)((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] + // h (x,1)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 2))[2] + //b (x,0)
                                        (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 2))[2]); //c (x+1,0)

                    int sumRedAbsCantoInfEsq = Math.Abs(redSumCantoInfEsqX) + Math.Abs(redSumCantoInfEsqY);

                    if (sumRedAbsCantoInfEsq > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if (sumRedAbsCantoInfEsq <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = (byte)sumRedAbsCantoInfEsq;
                    }

                }
                {
                    //canto inferior direito

                    int blueSumCantoInfDirX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[0] + //a
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[0] + //d
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[0]) - //g
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[0] + //c
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[0] + //f
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[0]); //i

                    int blueSumCantoInfDirY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[0] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[0] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[0]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[0] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[0] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[0]); //c (x+1,0)

                    int sumBlueAbsCantoInfDir = Math.Abs(blueSumCantoInfDirX) + Math.Abs(blueSumCantoInfDirY);

                    if (sumBlueAbsCantoInfDir > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if (sumBlueAbsCantoInfDir <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = (byte)sumBlueAbsCantoInfDir;
                    }

                    int greenSumCantoInfDirX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[1] + //a
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[1] + //d
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[1]) - //g
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[1] + //c
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[1] + //f
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[1]); //i

                    int greenSumCantoInfDirY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[1] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[1] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[1]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[1] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[1] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[1]); //c (x+1,0)

                    int sumGreenAbsCantoInfDir = Math.Abs(greenSumCantoInfDirX) + Math.Abs(greenSumCantoInfDirY);

                    if (sumGreenAbsCantoInfDir > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if (sumGreenAbsCantoInfDir <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = (byte)sumGreenAbsCantoInfDir;
                    }

                    int redSumCantoInfDirX = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[2] + //a
                                                2 * (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[2] + //d
                                                (dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[2]) - //g
                                                ((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[2] + //c
                                                2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[2] + //f
                                                (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[2]); //i

                    int redSumCantoInfDirY = (int)((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 1))[2] + // g (x-1,1)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[2] + // h (x,1)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 1))[2]) - //i (x+1,1)
                                        ((dataPtrCopy + nChan * (width - 2) + mCopy.widthStep * (height - 2))[2] + //a (x-1, 0)
                                        2 * (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[2] + //b (x,0)
                                        (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (height - 2))[2]); //c (x+1,0)

                    int sumRedAbsCantoInfDir = Math.Abs(redSumCantoInfDirX) + Math.Abs(redSumCantoInfDirY);

                    if (sumRedAbsCantoInfDir > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if (sumRedAbsCantoInfDir <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = (byte)sumRedAbsCantoInfDir;
                    }

                }
            }
        }

        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();           // pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();   // pointer to the image copy

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels (3 for RGB)
                int padding = m.widthStep - nChan * width;

                for (int y = 0; y < height - 1; y++)
                {
                    for (int x = 0; x < width - 1; x++)
                    {

                        int blueDiffX = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[0] -
                                             (dataPtr + nChan * (x + 1) + m.widthStep * y)[0]);

                        int blueDiffY = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[0] -
                                             (dataPtr + nChan * x + m.widthStep * (y + 1))[0]);


                        int sumBlueDiffAbs = blueDiffX + blueDiffY;

                        // Clamp para [0, 255]
                        if (sumBlueDiffAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 255;
                        }
                        else if (sumBlueDiffAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = (byte)sumBlueDiffAbs;
                        }


                        int greenDiffX = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[1] -
                                         (dataPtr + nChan * (x + 1) + m.widthStep * y)[1]);

                        int greenDiffY = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[1] -
                                         (dataPtr + nChan * x + m.widthStep * (y + 1))[1]);

                        int sumGreenDiffAbs = greenDiffX + greenDiffY;

                        if (sumGreenDiffAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 255;
                        }
                        else if (sumGreenDiffAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = (byte)sumGreenDiffAbs;
                        }


                        int redDiffX = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[2] -
                                         (dataPtr + nChan * (x + 1) + m.widthStep * y)[2]);

                        int redDiffY = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[2] -
                                         (dataPtr + nChan * x + m.widthStep * (y + 1))[2]);

                        int sumRedDiffAbs = redDiffX + redDiffY;

                        if (sumRedDiffAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 255;
                        }
                        else if (sumRedDiffAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = (byte)sumRedDiffAbs;
                        }
                    }
                }

                for (int x = 0; x < width - 1; x++)
                {
                    //não se faz a primeira linha
                    //linha baixo

                    int blueDiffLastLineX = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0] -
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0]);

                    int blueDiffLastLineY = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0] -
                                         (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0]);


                    int sumBlueDiffLastLineAbs = blueDiffLastLineX + blueDiffLastLineY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffLastLineAbs > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if (sumBlueDiffLastLineAbs <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = (byte)sumBlueDiffLastLineAbs;
                    }//blue

                    int greenDiffLastLineX = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1] -
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1]);

                    int greenDiffLastLineY = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1] -
                                         (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1]);


                    int sumGreenDiffLastLineAbs = greenDiffLastLineX + greenDiffLastLineY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffLastLineAbs > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if (sumGreenDiffLastLineAbs <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = (byte)sumGreenDiffLastLineAbs;
                    }//green

                    int redDiffLastLineX = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2] -
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2]);

                    int redDiffLastLineY = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2] -
                                         (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2]);


                    int sumRedDiffLastLineAbs = redDiffLastLineX + redDiffLastLineY;

                    // Clamp para [0, 255]
                    if (sumRedDiffLastLineAbs > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if (sumRedDiffLastLineAbs <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = (byte)sumRedDiffLastLineAbs;
                    }//red
                }

                for (int y = 0; y < height - 1; y++)
                {
                    //não se faz a primeira coluna
                    //ultima coluna

                    int blueDiffLastColX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0] -
                                             (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0]);

                    int blueDiffLastColY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0]);


                    int sumBlueDiffLastColAbs = blueDiffLastColX + blueDiffLastColY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffLastColAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 255;
                    }
                    else if (sumBlueDiffLastColAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = (byte)sumBlueDiffLastColAbs;
                    }//blue

                    int greenDiffLastColX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1] -
                                             (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1]);

                    int greenDiffLastColY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1]);


                    int sumGreenDiffLastColAbs = greenDiffLastColX + greenDiffLastColY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffLastColAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 255;
                    }
                    else if (sumGreenDiffLastColAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = (byte)sumGreenDiffLastColAbs;
                    }//green

                    int redDiffLastColX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2] -
                                             (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2]);

                    int redDiffLastColY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2]);


                    int sumRedDiffLastColAbs = redDiffLastColX + redDiffLastColY;

                    // Clamp para [0, 255]
                    if (sumRedDiffLastColAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 255;
                    }
                    else if (sumRedDiffLastColAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = (byte)sumRedDiffLastColAbs;
                    }//red                

                }
                {
                    //canto superior esquerdo

                    int blueDiffCantoSupEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] -
                                             (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[0]);

                    int blueDiffCantoSupEsqY = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] -
                                         (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[0]);


                    int sumBlueDiffCantoSupEsqAbs = blueDiffCantoSupEsqX + blueDiffCantoSupEsqY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffCantoSupEsqAbs > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueDiffCantoSupEsqAbs <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = (byte)sumBlueDiffCantoSupEsqAbs;
                    }//blue

                    int greenDiffCantoSupEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[1]);

                    int greenDiffCantoSupEsqY = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] -
                                         (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[1]);


                    int sumGreenDiffCantoSupEsqAbs = greenDiffCantoSupEsqX + greenDiffCantoSupEsqY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffCantoSupEsqAbs > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenDiffCantoSupEsqAbs <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = (byte)sumGreenDiffCantoSupEsqAbs;
                    }//green

                    int redDiffCantoSupEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[2]);

                    int redDiffCantoSupEsqY = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] -
                                         (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[2]);


                    int sumRedDiffCantoSupEsqAbs = redDiffCantoSupEsqX + redDiffCantoSupEsqY;

                    // Clamp para [0, 255]
                    if (sumRedDiffCantoSupEsqAbs > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedDiffCantoSupEsqAbs <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = (byte)sumRedDiffCantoSupEsqAbs;
                    }//red
                }
                {
                    //canto superior direito

                    int blueDiffCantoSupDirX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] -
                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0]);

                    int blueDiffCantoSupDirY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0]);


                    int sumBlueDiffCantoSupDirAbs = blueDiffCantoSupDirX + blueDiffCantoSupDirY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffCantoSupDirAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueDiffCantoSupDirAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = (byte)sumBlueDiffCantoSupDirAbs;
                    }//blue

                    int greenDiffCantoSupDirX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] -
                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1]);

                    int greenDiffCantoSupDirY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1]);


                    int sumGreenDiffCantoSupDirAbs = greenDiffCantoSupDirX + greenDiffCantoSupDirY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffCantoSupDirAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenDiffCantoSupDirAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = (byte)sumGreenDiffCantoSupDirAbs;
                    }//green

                    int redDiffCantoSupDirX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] -
                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2]);

                    int redDiffCantoSupDirY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2]);


                    int sumRedDiffCantoSupDirAbs = redDiffCantoSupDirX + redDiffCantoSupDirY;

                    // Clamp para [0, 255]
                    if (sumRedDiffCantoSupDirAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedDiffCantoSupDirAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = (byte)sumRedDiffCantoSupDirAbs;
                    }//red

                    {
                        //canto inferior esquerdo

                        int blueDiffCantoInfEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0]);

                        int blueDiffCantoInfEsqY = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] -
                                             (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0]);


                        int sumBlueDiffCantoInfEsqAbs = blueDiffCantoInfEsqX + blueDiffCantoSupDirY;

                        // Clamp para [0, 255]
                        if (sumBlueDiffCantoInfEsqAbs > 255)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 255;
                        }
                        else if (sumBlueDiffCantoInfEsqAbs <= 0)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = (byte)sumBlueDiffCantoInfEsqAbs;
                        }//blue

                        int greenDiffCantoInfEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1]);

                        int greenDiffCantoInfEsqY = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] -
                                             (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1]);


                        int sumGreenDiffCantoInfEsqAbs = greenDiffCantoInfEsqX + greenDiffCantoInfEsqY;

                        // Clamp para [0, 255]
                        if (sumGreenDiffCantoInfEsqAbs > 255)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 255;
                        }
                        else if (sumGreenDiffCantoInfEsqAbs <= 0)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = (byte)sumGreenDiffCantoInfEsqAbs;
                        }//green

                        int redDiffCantoInfEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2]);

                        int redDiffCantoInfEsqY = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] -
                                             (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2]);


                        int sumRedDiffCantoInfEsqAbs = redDiffCantoInfEsqX + redDiffCantoInfEsqY;

                        // Clamp para [0, 255]
                        if (sumRedDiffCantoInfEsqAbs > 255)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 255;
                        }
                        else if (sumRedDiffCantoInfEsqAbs <= 0)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = (byte)sumRedDiffCantoInfEsqAbs;
                        }//red
                    }
                    {
                        //canto inferior direito

                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = 0;
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = 0;
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = 0;
                    }
                }
            }
        }

        public static void Roberts(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();           // pointer to the image
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();   // pointer to the image copy

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = m.nChannels; // number of channels (3 for RGB)
                int padding = m.widthStep - nChan * width;

                for (int y = 0; y < height - 1; y++)
                {
                    for (int x = 0; x < width - 1; x++)
                    {

                        int blueDiffX = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[0] -
                                             (dataPtr + nChan * (x + 1) + m.widthStep * (y + 1))[0]);

                        int blueDiffY = Math.Abs((dataPtr + nChan * (x + 1) + m.widthStep * y)[0] -
                                             (dataPtr + nChan * x + m.widthStep * (y + 1))[0]);


                        int sumBlueDiffAbs = blueDiffX + blueDiffY;

                        // Clamp para [0, 255]
                        if (sumBlueDiffAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 255;
                        }
                        else if (sumBlueDiffAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = (byte)sumBlueDiffAbs;
                        }


                        int greenDiffX = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[1] -
                                         (dataPtr + nChan * (x + 1) + m.widthStep * (y + 1))[1]);

                        int greenDiffY = Math.Abs((dataPtr + nChan * (x + 1) + m.widthStep * y)[1] -
                                         (dataPtr + nChan * x + m.widthStep * (y + 1))[1]);

                        int sumGreenDiffAbs = greenDiffX + greenDiffY;

                        if (sumGreenDiffAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 255;
                        }
                        else if (sumGreenDiffAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[1] = (byte)sumGreenDiffAbs;
                        }


                        int redDiffX = Math.Abs((dataPtr + nChan * x + m.widthStep * y)[2] -
                                         (dataPtr + nChan * (x + 1) + m.widthStep * (y + 1))[2]);

                        int redDiffY = Math.Abs((dataPtr + nChan * (x + 1) + m.widthStep * y)[2] -
                                         (dataPtr + nChan * x + m.widthStep * (y + 1))[2]);

                        int sumRedDiffAbs = redDiffX + redDiffY;

                        if (sumRedDiffAbs > 255)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 255;
                        }
                        else if (sumRedDiffAbs <= 0)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[2] = (byte)sumRedDiffAbs;
                        }
                    }
                }

                for (int x = 0; x < width - 1; x++)
                {
                    //não se faz a primeira linha
                    //linha baixo

                    int blueDiffLastLineX = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0] -
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0]);

                    int blueDiffLastLineY = Math.Abs((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[0] -
                                         (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[0]);


                    int sumBlueDiffLastLineAbs = blueDiffLastLineX + blueDiffLastLineY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffLastLineAbs > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 255;
                    }
                    else if (sumBlueDiffLastLineAbs <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[0] = (byte)sumBlueDiffLastLineAbs;
                    }//blue

                    int greenDiffLastLineX = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1] -
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1]);

                    int greenDiffLastLineY = Math.Abs((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[1] -
                                         (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[1]);


                    int sumGreenDiffLastLineAbs = greenDiffLastLineX + greenDiffLastLineY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffLastLineAbs > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 255;
                    }
                    else if (sumGreenDiffLastLineAbs <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[1] = (byte)sumGreenDiffLastLineAbs;
                    }//green

                    int redDiffLastLineX = Math.Abs((dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2] -
                                             (dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2]);

                    int redDiffLastLineY = Math.Abs((dataPtrCopy + nChan * (x + 1) + mCopy.widthStep * (height - 1))[2] -
                                         (dataPtrCopy + nChan * x + mCopy.widthStep * (height - 1))[2]);


                    int sumRedDiffLastLineAbs = redDiffLastLineX + redDiffLastLineY;

                    // Clamp para [0, 255]
                    if (sumRedDiffLastLineAbs > 255)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 255;
                    }
                    else if (sumRedDiffLastLineAbs <= 0)
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * x + m.widthStep * (height - 1))[2] = (byte)sumRedDiffLastLineAbs;
                    }//red
                }

                for (int y = 0; y < height - 1; y++)
                {
                    //não se faz a primeira coluna
                    //ultima coluna

                    int blueDiffLastColX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0] -
                                             (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0]);

                    int blueDiffLastColY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[0] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[0]);


                    int sumBlueDiffLastColAbs = blueDiffLastColX + blueDiffLastColY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffLastColAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 255;
                    }
                    else if (sumBlueDiffLastColAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[0] = (byte)sumBlueDiffLastColAbs;
                    }//blue

                    int greenDiffLastColX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1] -
                                             (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1]);

                    int greenDiffLastColY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[1] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[1]);


                    int sumGreenDiffLastColAbs = greenDiffLastColX + greenDiffLastColY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffLastColAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 255;
                    }
                    else if (sumGreenDiffLastColAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[1] = (byte)sumGreenDiffLastColAbs;
                    }//green

                    int redDiffLastColX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2] -
                                             (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2]);

                    int redDiffLastColY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * y)[2] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * (y + 1))[2]);


                    int sumRedDiffLastColAbs = redDiffLastColX + redDiffLastColY;

                    // Clamp para [0, 255]
                    if (sumRedDiffLastColAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 255;
                    }
                    else if (sumRedDiffLastColAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * y)[2] = (byte)sumRedDiffLastColAbs;
                    }//red                

                }
                {
                    //canto superior esquerdo

                    int blueDiffCantoSupEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[0] -
                                             (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[0]);

                    int blueDiffCantoSupEsqY = Math.Abs((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[0] -
                                         (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[0]);


                    int sumBlueDiffCantoSupEsqAbs = blueDiffCantoSupEsqX + blueDiffCantoSupEsqY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffCantoSupEsqAbs > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueDiffCantoSupEsqAbs <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[0] = (byte)sumBlueDiffCantoSupEsqAbs;
                    }//blue

                    int greenDiffCantoSupEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[1] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[1]);

                    int greenDiffCantoSupEsqY = Math.Abs((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[1] -
                                         (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[1]);


                    int sumGreenDiffCantoSupEsqAbs = greenDiffCantoSupEsqX + greenDiffCantoSupEsqY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffCantoSupEsqAbs > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenDiffCantoSupEsqAbs <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[1] = (byte)sumGreenDiffCantoSupEsqAbs;
                    }//green

                    int redDiffCantoSupEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * 0)[2] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * 1)[2]);

                    int redDiffCantoSupEsqY = Math.Abs((dataPtrCopy + nChan * 1 + mCopy.widthStep * 0)[2] -
                                         (dataPtrCopy + nChan * 0 + mCopy.widthStep * 1)[2]);


                    int sumRedDiffCantoSupEsqAbs = redDiffCantoSupEsqX + redDiffCantoSupEsqY;

                    // Clamp para [0, 255]
                    if (sumRedDiffCantoSupEsqAbs > 255)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedDiffCantoSupEsqAbs <= 0)
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * 0 + m.widthStep * 0)[2] = (byte)sumRedDiffCantoSupEsqAbs;
                    }//red
                }
                {
                    //canto superior direito

                    int blueDiffCantoSupDirX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] -
                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0]);

                    int blueDiffCantoSupDirY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[0] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[0]);


                    int sumBlueDiffCantoSupDirAbs = blueDiffCantoSupDirX + blueDiffCantoSupDirY;

                    // Clamp para [0, 255]
                    if (sumBlueDiffCantoSupDirAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 255;
                    }
                    else if (sumBlueDiffCantoSupDirAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[0] = (byte)sumBlueDiffCantoSupDirAbs;
                    }//blue

                    int greenDiffCantoSupDirX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] -
                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1]);

                    int greenDiffCantoSupDirY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[1] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[1]);


                    int sumGreenDiffCantoSupDirAbs = greenDiffCantoSupDirX + greenDiffCantoSupDirY;

                    // Clamp para [0, 255]
                    if (sumGreenDiffCantoSupDirAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 255;
                    }
                    else if (sumGreenDiffCantoSupDirAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[1] = (byte)sumGreenDiffCantoSupDirAbs;
                    }//green

                    int redDiffCantoSupDirX = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] -
                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2]);

                    int redDiffCantoSupDirY = Math.Abs((dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 0)[2] -
                                         (dataPtrCopy + nChan * (width - 1) + mCopy.widthStep * 1)[2]);


                    int sumRedDiffCantoSupDirAbs = redDiffCantoSupDirX + redDiffCantoSupDirY;

                    // Clamp para [0, 255]
                    if (sumRedDiffCantoSupDirAbs > 255)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 255;
                    }
                    else if (sumRedDiffCantoSupDirAbs <= 0)
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = 0;
                    }
                    else
                    {
                        (dataPtr + nChan * (width - 1) + m.widthStep * 0)[2] = (byte)sumRedDiffCantoSupDirAbs;
                    }//red

                    {
                        //canto inferior esquerdo

                        int blueDiffCantoInfEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0]);

                        int blueDiffCantoInfEsqY = Math.Abs((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[0] -
                                             (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[0]);


                        int sumBlueDiffCantoInfEsqAbs = blueDiffCantoInfEsqX + blueDiffCantoSupDirY;

                        // Clamp para [0, 255]
                        if (sumBlueDiffCantoInfEsqAbs > 255)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 255;
                        }
                        else if (sumBlueDiffCantoInfEsqAbs <= 0)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[0] = (byte)sumBlueDiffCantoInfEsqAbs;
                        }//blue

                        int greenDiffCantoInfEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1]);

                        int greenDiffCantoInfEsqY = Math.Abs((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[1] -
                                             (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[1]);


                        int sumGreenDiffCantoInfEsqAbs = greenDiffCantoInfEsqX + greenDiffCantoInfEsqY;

                        // Clamp para [0, 255]
                        if (sumGreenDiffCantoInfEsqAbs > 255)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 255;
                        }
                        else if (sumGreenDiffCantoInfEsqAbs <= 0)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[1] = (byte)sumGreenDiffCantoInfEsqAbs;
                        }//green

                        int redDiffCantoInfEsqX = Math.Abs((dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2] -
                         (dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2]);

                        int redDiffCantoInfEsqY = Math.Abs((dataPtrCopy + nChan * 1 + mCopy.widthStep * (height - 1))[2] -
                                             (dataPtrCopy + nChan * 0 + mCopy.widthStep * (height - 1))[2]);


                        int sumRedDiffCantoInfEsqAbs = redDiffCantoInfEsqX + redDiffCantoInfEsqY;

                        // Clamp para [0, 255]
                        if (sumRedDiffCantoInfEsqAbs > 255)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 255;
                        }
                        else if (sumRedDiffCantoInfEsqAbs <= 0)
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = 0;
                        }
                        else
                        {
                            (dataPtr + nChan * 0 + m.widthStep * (height - 1))[2] = (byte)sumRedDiffCantoInfEsqAbs;
                        }//red
                    }
                    {
                        //canto inferior direito

                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[0] = 0;
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[1] = 0;
                        (dataPtr + nChan * (width - 1) + m.widthStep * (height - 1))[2] = 0;
                    }
                }
            }
        }

        public static void Median(Image<Bgr, byte> inputImage, Image<Bgr, byte> outputImage)
        {
            unsafe
            {
                MIplImage inputMipl = inputImage.MIplImage;
                MIplImage outputMipl = outputImage.MIplImage;
                byte* inputPtr = (byte*)inputMipl.imageData.ToPointer();         // Pointer to the input image data
                byte* outputPtr = (byte*)outputMipl.imageData.ToPointer();       // Pointer to the output image data

                int imageWidth = outputImage.Width;
                int imageHeight = outputImage.Height;
                int numChannels = inputMipl.nChannels; // number of color channels (3 for RGB)
                int padding = inputMipl.widthStep - numChannels * imageWidth;

                // Temporary storage for calculating color distances and storing pixel colors
                double[] colorDistances = new double[9];
                Bgr[] neighborColors = new Bgr[9];

                // Loop through each pixel in the image
                for (int y = 0; y < imageHeight; y++)
                {
                    for (int x = 0; x < imageWidth; x++)
                    {
                        int neighborIndex = 0;
                        int validNeighbors = 0;

                        // Collect all possible neighbors within 3x3 window around the current pixel
                        for (int offsetY = -1; offsetY <= 1; offsetY++)
                        {
                            for (int offsetX = -1; offsetX <= 1; offsetX++)
                            {
                                int neighborX = x + offsetX;
                                int neighborY = y + offsetY;
                                // Ensure neighbor indices are within image bounds
                                if (neighborX >= 0 && neighborY >= 0 && neighborX < imageWidth && neighborY < imageHeight)
                                {
                                    byte* neighborPtr = outputPtr + (neighborY * inputMipl.widthStep) + (neighborX * numChannels);
                                    neighborColors[neighborIndex] = new Bgr(neighborPtr[0], neighborPtr[1], neighborPtr[2]);
                                    colorDistances[neighborIndex] = 0; // Initialize distance
                                    neighborIndex++;
                                    validNeighbors++;
                                }
                            }
                        }

                        // Calculate sum of distances for each collected neighbor color
                        for (int i = 0; i < validNeighbors; i++)
                        {
                            double sumDistance = 0;
                            for (int j = 0; j < validNeighbors; j++)
                            {
                                if (i != j)
                                {
                                    sumDistance += Math.Sqrt(
                                        Math.Pow(neighborColors[i].Blue - neighborColors[j].Blue, 2) +
                                        Math.Pow(neighborColors[i].Green - neighborColors[j].Green, 2) +
                                        Math.Pow(neighborColors[i].Red - neighborColors[j].Red, 2));
                                }
                            }
                            colorDistances[i] = sumDistance;
                        }

                        // Identify the neighbor with the minimum total color distance
                        int minDistanceIndex = 0;
                        double minimumDistance = colorDistances[0];
                        for (int i = 1; i < validNeighbors; i++)
                        {
                            if (colorDistances[i] < minimumDistance)
                            {
                                minimumDistance = colorDistances[i];
                                minDistanceIndex = i;
                            }
                        }

                        // Set the current pixel to the color of the neighbor with the minimum distance
                        byte* currentPixel = inputPtr + (y * inputMipl.widthStep) + (x * numChannels);
                        currentPixel[0] = (byte)neighborColors[minDistanceIndex].Blue;
                        currentPixel[1] = (byte)neighborColors[minDistanceIndex].Green;
                        currentPixel[2] = (byte)neighborColors[minDistanceIndex].Red;
                    }
                }
            }
        }

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image



                byte blue, green, red;
                int Gray;
                int[] hist = new int[256];

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y, x0, y0;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {

                            blue = (dataPtr + x * nChan + y * m.widthStep)[0];
                            green = (dataPtr + x * nChan + y * m.widthStep)[1];
                            red = (dataPtr + x * nChan + y * m.widthStep)[2];
                            //retrive 3 colour components
                            Gray = (int)Math.Round((red + green + blue) / 3.0);
                            hist[Gray]++;
                            // store in the image

                        }

                    }
                }
                Histograma h = new Histograma(hist);
                h.ShowDialog();
                return hist;
            }
        }
        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // Acesso direto à memória da imagem (sequencial)
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Ponteiro para a imagem

                // Inicializa o histograma para cada canal RGB
                int[,] histogram = new int[3, 256];

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // número de canais - 3
                int padding = m.widthStep - m.nChannels * m.width; // bytes de alinhamento (padding)
                int x, y;

                if (nChan == 3) // imagem em RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Recupera os três componentes de cor
                            byte blue = (dataPtr + x * nChan + y * m.widthStep)[0];
                            byte green = (dataPtr + x * nChan + y * m.widthStep)[1];
                            byte red = (dataPtr + x * nChan + y * m.widthStep)[2];

                            // Incrementa o histograma para cada canal
                            histogram[0, blue]++; // Canal Azul
                            histogram[1, green]++; // Canal Verde
                            histogram[2, red]++; // Canal Vermelho
                        }
                    }
                }
                Histograma h = new Histograma(histogram);
                h.ShowDialog();
                return histogram;
            }
        }
        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                // Acesso direto à memória da imagem (sequencial)
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Ponteiro para a imagem

                // Inicializa o histograma para escala de cinza e os três canais RGB
                int[,] histogram = new int[4, 256];

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // número de canais - 3
                int padding = m.widthStep - m.nChannels * m.width; // bytes de alinhamento (padding)
                int x, y;

                if (nChan == 3) // imagem em RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // Recupera os três componentes de cor
                            byte blue = (dataPtr + x * nChan + y * m.widthStep)[0];
                            byte green = (dataPtr + x * nChan + y * m.widthStep)[1];
                            byte red = (dataPtr + x * nChan + y * m.widthStep)[2];

                            // Calcula a intensidade em escala de cinza usando a média dos três canais
                            int gray = (int)Math.Round((red + green + blue) / 3.0);

                            // Incrementa o histograma em escala de cinza
                            histogram[0, gray]++;

                            // Incrementa o histograma para cada canal de cor
                            histogram[1, blue]++; // Canal Azul
                            histogram[2, green]++; // Canal Verde
                            histogram[3, red]++; // Canal Vermelho
                        }
                    }
                }

                // Exibe os histogramas em uma nova janela
                Histograma h = new Histograma(histogram);
                h.ShowDialog();

                return histogram;
            }
        }
        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte blue, green, red;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte bluech = (dataPtr + nChan * x + m.widthStep * y)[0];
                        byte greench = (dataPtr + nChan * x + m.widthStep * y)[1];
                        byte redch = (dataPtr + nChan * x + m.widthStep * y)[2];

                        int valor = (int)Math.Round((int)(bluech + greench + redch) / 3.0);



                        if (valor > threshold)
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = (dataPtr + nChan * x + m.widthStep * y)[1] = (dataPtr + nChan * x + m.widthStep * y)[2] = 255;
                        }
                        else
                        {
                            (dataPtr + nChan * x + m.widthStep * y)[0] = (dataPtr + nChan * x + m.widthStep * y)[1] = (dataPtr + nChan * x + m.widthStep * y)[2] = 0;
                        }

                    }
                }
            }
        }




        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int[] hist = Histogram_Gray(img);
                int total = width * height;

                double[] q1 = new double[256];
                double[] q2 = new double[256];
                double[] varianca = new double[256];
                double[] u1 = new double[256];
                double[] u2 = new double[256];

                if (nChan == 3)
                {

                    for (int t = 0; t <= 255; t++)
                    {
                        if (t == 0)
                        {
                            q1[t] = (double)hist[t] / total;
                            u1[t] = q1[t] * t;
                        }
                        else
                        {
                            q1[t] = q1[t - 1] + (double)hist[t] / total;
                            u1[t] = u1[t - 1] + (double)hist[t] * t / total;
                        }
                    }


                    for (int t = 254; t >= 0; t--)
                    {
                        if (t == 254)
                        {
                            q2[t] = (double)hist[255] / total;
                            u2[t] = q2[t] * 255;
                        }
                        else
                        {
                            q2[t] = q2[t + 1] + (double)hist[t + 1] / total;
                            u2[t] = u2[t + 1] + (double)hist[t + 1] * (t + 1) / total;
                        }
                    }


                    for (int t = 0; t <= 255; t++)
                    {
                        if (q1[t] > 0 && q2[t] > 0)
                        {
                            double meanDiff = u1[t] / q1[t] - u2[t] / q2[t];
                            varianca[t] = q1[t] * q2[t] * meanDiff * meanDiff;
                        }
                        else
                        {
                            varianca[t] = 0;
                        }
                    }
                }


                int threshold = Array.IndexOf(varianca, varianca.Max());
                ConvertToBW(img, threshold);
            }
        }

        public static void Mean_solutionB(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrimgCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                int padding = widthStep - nChan * width;

                int last_h = height - 1;
                int last_w = width - 1;

                // Canto superior esquerdo (top-left corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy + nChan)[0] +
                                    2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + nChan + widthStep)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy + nChan)[1] +
                                    2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + nChan + widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy + nChan)[2] +
                                    2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + nChan + widthStep)[2]) / 9.0);

                dataPtrImg += nChan;
                dataPtrimgCopy += nChan;

                // Linha de cima (top row)
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[0] + 2 * dataPtrimgCopy[0] +
                                        2 * (dataPtrimgCopy + nChan)[0] + (dataPtrimgCopy - nChan + widthStep)[0] +
                                        (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + nChan + widthStep)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[1] + 2 * dataPtrimgCopy[1] +
                                        2 * (dataPtrimgCopy + nChan)[1] + (dataPtrimgCopy - nChan + widthStep)[1] +
                                        (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + nChan + widthStep)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[2] + 2 * dataPtrimgCopy[2] +
                                        2 * (dataPtrimgCopy + nChan)[2] + (dataPtrimgCopy - nChan + widthStep)[2] +
                                        (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + nChan + widthStep)[2]) / 9.0);

                    dataPtrImg += nChan;
                    dataPtrimgCopy += nChan;
                }

                // Canto superior direito (top-right corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy - nChan)[0] +
                                    2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy - nChan + widthStep)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy - nChan)[1] +
                                    2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy - nChan + widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy - nChan)[2] +
                                    2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy - nChan + widthStep)[2]) / 9.0);

                dataPtrImg += nChan + padding;
                dataPtrimgCopy += nChan + padding;

                // Process interior and sides of the image
                for (int y = 1; y < last_h; y++)
                {
                    // Margem esquerda (left margin)
                    dataPtrImg[0] = (byte)Math.Round((2 * dataPtrimgCopy[0] + (dataPtrimgCopy + nChan)[0] +
                                        2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + nChan + widthStep)[0] +
                                        2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * dataPtrimgCopy[1] + (dataPtrimgCopy + nChan)[1] +
                                        2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + nChan + widthStep)[1] +
                                        2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * dataPtrimgCopy[2] + (dataPtrimgCopy + nChan)[2] +
                                        2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + nChan + widthStep)[2] +
                                        2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2]) / 9.0);

                    dataPtrImg += nChan;
                    dataPtrimgCopy += nChan;

                    // Process internal pixels
                    for (int x = 1; x < last_w; x++)
                    {
                        dataPtrImg[0] = (byte)Math.Round((dataPtrimgCopy[0] + (dataPtrimgCopy + nChan)[0] +
                                            (dataPtrimgCopy - nChan)[0] + (dataPtrimgCopy + widthStep + nChan)[0] +
                                            (dataPtrimgCopy + widthStep - nChan)[0] + (dataPtrimgCopy + widthStep)[0] +
                                            (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0] +
                                            (dataPtrimgCopy - widthStep - nChan)[0]) / 9.0);
                        dataPtrImg[1] = (byte)Math.Round((dataPtrimgCopy[1] + (dataPtrimgCopy + nChan)[1] +
                                            (dataPtrimgCopy - nChan)[1] + (dataPtrimgCopy + widthStep + nChan)[1] +
                                            (dataPtrimgCopy + widthStep - nChan)[1] + (dataPtrimgCopy + widthStep)[1] +
                                            (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1] +
                                            (dataPtrimgCopy - widthStep - nChan)[1]) / 9.0);
                        dataPtrImg[2] = (byte)Math.Round((dataPtrimgCopy[2] + (dataPtrimgCopy + nChan)[2] +
                                            (dataPtrimgCopy - nChan)[2] + (dataPtrimgCopy + widthStep + nChan)[2] +
                                            (dataPtrimgCopy + widthStep - nChan)[2] + (dataPtrimgCopy + widthStep)[2] +
                                            (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2] +
                                            (dataPtrimgCopy - widthStep - nChan)[2]) / 9.0);

                        dataPtrImg += nChan;
                        dataPtrimgCopy += nChan;
                    }

                    // Margem direita (right margin)
                    dataPtrImg[0] = (byte)Math.Round((2 * dataPtrimgCopy[0] + (dataPtrimgCopy - nChan)[0] +
                                        2 * (dataPtrimgCopy + widthStep)[0] + (dataPtrimgCopy + widthStep - nChan)[0] +
                                        2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep - nChan)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * dataPtrimgCopy[1] + (dataPtrimgCopy - nChan)[1] +
                                        2 * (dataPtrimgCopy + widthStep)[1] + (dataPtrimgCopy + widthStep - nChan)[1] +
                                        2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep - nChan)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * dataPtrimgCopy[2] + (dataPtrimgCopy - nChan)[2] +
                                        2 * (dataPtrimgCopy + widthStep)[2] + (dataPtrimgCopy + widthStep - nChan)[2] +
                                        2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep - nChan)[2]) / 9.0);

                    dataPtrImg += nChan + padding;
                    dataPtrimgCopy += nChan + padding;
                }

                // Canto inferior esquerdo (bottom-left corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy + nChan)[0] +
                                    2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy + nChan)[1] +
                                    2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy + nChan)[2] +
                                    2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2]) / 9.0);

                dataPtrImg += nChan;
                dataPtrimgCopy += nChan;

                // Linha de baixo (bottom row)
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[0] + 2 * dataPtrimgCopy[0] +
                                        2 * (dataPtrimgCopy + nChan)[0] + (dataPtrimgCopy - widthStep - nChan)[0] +
                                        (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep + nChan)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[1] + 2 * dataPtrimgCopy[1] +
                                        2 * (dataPtrimgCopy + nChan)[1] + (dataPtrimgCopy - widthStep - nChan)[1] +
                                        (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep + nChan)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtrimgCopy - nChan)[2] + 2 * dataPtrimgCopy[2] +
                                        2 * (dataPtrimgCopy + nChan)[2] + (dataPtrimgCopy - widthStep - nChan)[2] +
                                        (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep + nChan)[2]) / 9.0);

                    dataPtrImg += nChan;
                    dataPtrimgCopy += nChan;
                }

                // Canto inferior direito (bottom-right corner)
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtrimgCopy[0] + 2 * (dataPtrimgCopy - nChan)[0] +
                                    2 * (dataPtrimgCopy - widthStep)[0] + (dataPtrimgCopy - widthStep - nChan)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtrimgCopy[1] + 2 * (dataPtrimgCopy - nChan)[1] +
                                    2 * (dataPtrimgCopy - widthStep)[1] + (dataPtrimgCopy - widthStep - nChan)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtrimgCopy[2] + 2 * (dataPtrimgCopy - nChan)[2] +
                            2 * (dataPtrimgCopy - widthStep)[2] + (dataPtrimgCopy - widthStep - nChan)[2]) / 9.0);
            }
        }

        public static void Mean_solutionC(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int size)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrimgCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthStep = mUndo.widthStep;
                int padding = widthStep - nChan * width;

                int radius = size / 2;
                int windowSize = size * size;

                // Process cada pixel da imagem
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Process cada canal de cor
                        for (int c = 0; c < nChan; c++)
                        {
                            float sum = 0;
                            int count = 0;

                            // Calcula a média 7x7 para cada canal
                            for (int ky = -radius; ky <= radius; ky++)
                            {
                                int py = y + ky;
                                if (py < 0 || py >= height) continue;

                                byte* rowPtr = dataPtrimgCopy + py * widthStep;

                                for (int kx = -radius; kx <= radius; kx++)
                                {
                                    int px = x + kx;
                                    if (px < 0 || px >= width) continue;

                                    sum += rowPtr[px * nChan + c];
                                    count++;
                                }
                            }

                            // Aplica o resultado da média
                            dataPtrImg[y * widthStep + x * nChan + c] = (byte)Math.Round(sum / count);
                        }
                    }
                }
            }
        }

        public static void Rotation_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                MIplImage m_copy = imgCopy.MIplImage;
                byte* dataPtr_copy = (byte*)m_copy.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                double cos_angle = Math.Cos(angle);
                double sen_angle = Math.Sin(angle);

                if (nChan == 3)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Calcula as coordenadas do pixel na imagem original
                            double x_0 = (x - width / 2.0) * cos_angle - (height / 2.0 - y) * sen_angle + width / 2.0;
                            double y_0 = height / 2.0 - (x - width / 2.0) * sen_angle - (height / 2.0 - y) * cos_angle;

                            // Pega as coordenadas dos quatro pixels vizinhos
                            int x1 = (int)Math.Floor(x_0);
                            int y1 = (int)Math.Floor(y_0);
                            int x2 = x1 + 1;
                            int y2 = y1 + 1;

                            // Calcula os pesos para interpolação
                            double dx = x_0 - x1;
                            double dy = y_0 - y1;

                            byte blue = 0, green = 0, red = 0;

                            // Verifica se os pontos estão dentro da imagem
                            if (x1 >= 0 && x2 < width && y1 >= 0 && y2 < height)
                            {
                                // Pega os valores dos quatro pixels vizinhos para cada canal
                                byte b11, b12, b21, b22;
                                byte g11, g12, g21, g22;
                                byte r11, r12, r21, r22;

                                // Pixel superior esquerdo (x1,y1)
                                b11 = (dataPtr_copy + y1 * m.widthStep + x1 * nChan)[0];
                                g11 = (dataPtr_copy + y1 * m.widthStep + x1 * nChan)[1];
                                r11 = (dataPtr_copy + y1 * m.widthStep + x1 * nChan)[2];

                                // Pixel superior direito (x2,y1)
                                b12 = (dataPtr_copy + y1 * m.widthStep + x2 * nChan)[0];
                                g12 = (dataPtr_copy + y1 * m.widthStep + x2 * nChan)[1];
                                r12 = (dataPtr_copy + y1 * m.widthStep + x2 * nChan)[2];

                                // Pixel inferior esquerdo (x1,y2)
                                b21 = (dataPtr_copy + y2 * m.widthStep + x1 * nChan)[0];
                                g21 = (dataPtr_copy + y2 * m.widthStep + x1 * nChan)[1];
                                r21 = (dataPtr_copy + y2 * m.widthStep + x1 * nChan)[2];

                                // Pixel inferior direito (x2,y2)
                                b22 = (dataPtr_copy + y2 * m.widthStep + x2 * nChan)[0];
                                g22 = (dataPtr_copy + y2 * m.widthStep + x2 * nChan)[1];
                                r22 = (dataPtr_copy + y2 * m.widthStep + x2 * nChan)[2];

                                // Interpolação bilinear para cada canal
                                blue = (byte)Math.Round(
                                    b11 * (1 - dx) * (1 - dy) +
                                    b12 * dx * (1 - dy) +
                                    b21 * (1 - dx) * dy +
                                    b22 * dx * dy
                                );

                                green = (byte)Math.Round(
                                    g11 * (1 - dx) * (1 - dy) +
                                    g12 * dx * (1 - dy) +
                                    g21 * (1 - dx) * dy +
                                    g22 * dx * dy
                                );

                                red = (byte)Math.Round(
                                    r11 * (1 - dx) * (1 - dy) +
                                    r12 * dx * (1 - dy) +
                                    r21 * (1 - dx) * dy +
                                    r22 * dx * dy
                                );
                            }

                            // Atribui os valores interpolados
                            dataPtr[0] = blue;
                            dataPtr[1] = green;
                            dataPtr[2] = red;

                            // Avança o ponteiro para o próximo pixel
                            dataPtr += nChan;
                        }
                        // No final da linha, avança o ponteiro pelos bytes de alinhamento (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void Scale_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mCopy = imgCopy.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Calcula as coordenadas de origem com ponto flutuante para maior precisão
                        double srcX = x / scaleFactor;
                        double srcY = y / scaleFactor;

                        // Encontra as coordenadas dos pixels vizinhos
                        int x1 = (int)Math.Floor(srcX);
                        int y1 = (int)Math.Floor(srcY);
                        int x2 = x1 + 1;
                        int y2 = y1 + 1;

                        // Calcula os pesos para interpolação
                        double dx = srcX - x1;
                        double dy = srcY - y1;

                        byte blue = 0, green = 0, red = 0;

                        // Verifica se os pontos estão dentro da imagem
                        if (x1 >= 0 && x2 < width && y1 >= 0 && y2 < height)
                        {
                            // Pega os valores dos quatro pixels vizinhos para cada canal
                            byte b11, b12, b21, b22;
                            byte g11, g12, g21, g22;
                            byte r11, r12, r21, r22;

                            // Pixel superior esquerdo (x1,y1)
                            b11 = (dataPtrCopy + y1 * m.widthStep + x1 * nChan)[0];
                            g11 = (dataPtrCopy + y1 * m.widthStep + x1 * nChan)[1];
                            r11 = (dataPtrCopy + y1 * m.widthStep + x1 * nChan)[2];

                            // Pixel superior direito (x2,y1)
                            b12 = (dataPtrCopy + y1 * m.widthStep + x2 * nChan)[0];
                            g12 = (dataPtrCopy + y1 * m.widthStep + x2 * nChan)[1];
                            r12 = (dataPtrCopy + y1 * m.widthStep + x2 * nChan)[2];

                            // Pixel inferior esquerdo (x1,y2)
                            b21 = (dataPtrCopy + y2 * m.widthStep + x1 * nChan)[0];
                            g21 = (dataPtrCopy + y2 * m.widthStep + x1 * nChan)[1];
                            r21 = (dataPtrCopy + y2 * m.widthStep + x1 * nChan)[2];

                            // Pixel inferior direito (x2,y2)
                            b22 = (dataPtrCopy + y2 * m.widthStep + x2 * nChan)[0];
                            g22 = (dataPtrCopy + y2 * m.widthStep + x2 * nChan)[1];
                            r22 = (dataPtrCopy + y2 * m.widthStep + x2 * nChan)[2];

                            // Interpolação bilinear para cada canal
                            blue = (byte)Math.Round(
                                b11 * (1 - dx) * (1 - dy) +
                                b12 * dx * (1 - dy) +
                                b21 * (1 - dx) * dy +
                                b22 * dx * dy
                            );

                            green = (byte)Math.Round(
                                g11 * (1 - dx) * (1 - dy) +
                                g12 * dx * (1 - dy) +
                                g21 * (1 - dx) * dy +
                                g22 * dx * dy
                            );

                            red = (byte)Math.Round(
                                r11 * (1 - dx) * (1 - dy) +
                                r12 * dx * (1 - dy) +
                                r21 * (1 - dx) * dy +
                                r22 * dx * dy
                            );
                        }
                        else
                        {
                            // Se estiver fora da imagem, coloca preto
                            blue = green = red = 0;
                        }

                        // Define os valores dos canais BGR no pixel escalado
                        dataPtr[0] = blue;
                        dataPtr[1] = green;
                        dataPtr[2] = red;

                        // Move para o próximo pixel
                        dataPtr += nChan;
                    }
                    // Saltar o padding no final de cada linha
                    dataPtr += padding;
                }
            }
        }

        public static void Scale_point_xy_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                MIplImage m2 = imgCopy.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int hlf_w = width / 2;
                int hlf_h = height / 2;

                if (scaleFactor != 0)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Calcula as coordenadas de origem com ponto flutuante
                            double srcX = ((x - hlf_w) / scaleFactor) + centerX;
                            double srcY = ((y - hlf_h) / scaleFactor) + centerY;

                            // Encontra as coordenadas dos pixels vizinhos
                            int x1 = (int)Math.Floor(srcX);
                            int y1 = (int)Math.Floor(srcY);
                            int x2 = x1 + 1;
                            int y2 = y1 + 1;

                            // Calcula os pesos para interpolação
                            double dx = srcX - x1;
                            double dy = srcY - y1;

                            // Valores padrão (preto)
                            byte blue = 0, green = 0, red = 0;

                            // Verifica se os pontos estão dentro da imagem
                            if (x1 >= 0 && x2 < width && y1 >= 0 && y2 < height)
                            {
                                // Pega os valores dos quatro pixels vizinhos para cada canal
                                byte b11, b12, b21, b22;
                                byte g11, g12, g21, g22;
                                byte r11, r12, r21, r22;

                                // Pixel superior esquerdo (x1,y1)
                                b11 = (dataPtr2 + y1 * m2.widthStep + x1 * nChan)[0];
                                g11 = (dataPtr2 + y1 * m2.widthStep + x1 * nChan)[1];
                                r11 = (dataPtr2 + y1 * m2.widthStep + x1 * nChan)[2];

                                // Pixel superior direito (x2,y1)
                                b12 = (dataPtr2 + y1 * m2.widthStep + x2 * nChan)[0];
                                g12 = (dataPtr2 + y1 * m2.widthStep + x2 * nChan)[1];
                                r12 = (dataPtr2 + y1 * m2.widthStep + x2 * nChan)[2];

                                // Pixel inferior esquerdo (x1,y2)
                                b21 = (dataPtr2 + y2 * m2.widthStep + x1 * nChan)[0];
                                g21 = (dataPtr2 + y2 * m2.widthStep + x1 * nChan)[1];
                                r21 = (dataPtr2 + y2 * m2.widthStep + x1 * nChan)[2];

                                // Pixel inferior direito (x2,y2)
                                b22 = (dataPtr2 + y2 * m2.widthStep + x2 * nChan)[0];
                                g22 = (dataPtr2 + y2 * m2.widthStep + x2 * nChan)[1];
                                r22 = (dataPtr2 + y2 * m2.widthStep + x2 * nChan)[2];

                                // Interpolação bilinear para cada canal
                                blue = (byte)Math.Round(
                                    b11 * (1 - dx) * (1 - dy) +
                                    b12 * dx * (1 - dy) +
                                    b21 * (1 - dx) * dy +
                                    b22 * dx * dy
                                );

                                green = (byte)Math.Round(
                                    g11 * (1 - dx) * (1 - dy) +
                                    g12 * dx * (1 - dy) +
                                    g21 * (1 - dx) * dy +
                                    g22 * dx * dy
                                );

                                red = (byte)Math.Round(
                                    r11 * (1 - dx) * (1 - dy) +
                                    r12 * dx * (1 - dy) +
                                    r21 * (1 - dx) * dy +
                                    r22 * dx * dy
                                );
                            }

                            // Armazena os valores interpolados na imagem
                            dataPtr[0] = blue;
                            dataPtr[1] = green;
                            dataPtr[2] = red;

                            // Avança o ponteiro para o próximo pixel
                            dataPtr += nChan;
                        }
                        // No final da linha, avança o ponteiro pelos bytes de alinhamento
                        dataPtr += padding;
                    }
                }
            }
        }




        //projeto
        public static void Binarizacao(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;

                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;
                int padding = widthStep - nChan * width;
                byte blue, green, red;

                int last_h = height - 1;
                int last_w = width - 1;


                blue = dataPtrImg[0];
                green = dataPtrImg[1];
                red = dataPtrImg[2];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {


                        if ((dataPtrImg + x * nChan + y * m.widthStep)[0] == blue && (dataPtrImg + x * nChan + y * m.widthStep)[1] == green && (dataPtrImg + x * nChan + y * m.widthStep)[2] == red)
                        {
                            (dataPtrImg + x * nChan + y * m.widthStep)[0] = 0;
                            (dataPtrImg + x * nChan + y * m.widthStep)[1] = 0;
                            (dataPtrImg + x * nChan + y * m.widthStep)[2] = 0;


                        }
                        else
                        {
                            (dataPtrImg + x * nChan + y * m.widthStep)[0] = 255;
                            (dataPtrImg + x * nChan + y * m.widthStep)[1] = 255;
                            (dataPtrImg + x * nChan + y * m.widthStep)[2] = 255;


                        }

                    }
                }
            }
        }


        public static List<int> Sorting(int[,] eti)
        {
            int height = eti.GetLength(0);
            int width = eti.GetLength(1);
            List<int> chave = new List<int>();

            // Adiciona a primeira etiqueta encontrada

            chave.Add(0);


            // Percorre a matriz de etiquetas
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int etiqueta = eti[y, x];
                    if (etiqueta != 0 && chave.Contains(etiqueta) == false)
                    {
                        chave.Add(etiqueta);  // Adiciona a etiqueta se não estiver presente na lista
                    }
                    else continue;
                }
            }

            // Ordena a lista de etiquetas
            chave.Sort();

            return chave;
        }


        public static int[,] EtiquetasFuncao(Image<Bgr, byte> img) 
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                int step = m.widthStep;
                int width = img.Width, height = img.Height;
                int[,] Etiquetas = new int[height, width];
                int nChan = m.nChannels;
                int numeroEtiqueta = 1;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                // Inicialização das etiquetas
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if ((dataPtr + nChan * x + step * y)[0] == 255 &&
                            (dataPtr + nChan * x + step * y)[1] == 255 &&
                            (dataPtr + nChan * x + step * y)[2] == 255)
                        {
                            Etiquetas[y, x] = numeroEtiqueta++;
                        }
                        else
                        {
                            Etiquetas[y, x] = 0;
                        }
                    }
                }

                bool alteracao = false;
                do
                {


                    // Passagem 1: cima para baixo, esquerda para direita
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (Etiquetas[y, x] != 0)
                            {
                                int menorEtiqueta = Etiquetas[y, x];

                                // Checar vizinhos com validação de limites
                                if (Etiquetas[y - 1, x - 1] < menorEtiqueta && Etiquetas[y - 1, x - 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y - 1, x - 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y - 1, x] < menorEtiqueta && Etiquetas[y - 1, x] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y - 1, x];
                                    alteracao = true;
                                }
                                if (Etiquetas[y - 1, x + 1] < menorEtiqueta && Etiquetas[y - 1, x + 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y - 1, x + 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y, x - 1] < menorEtiqueta && Etiquetas[y, x - 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y, x - 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y, x + 1] < menorEtiqueta && Etiquetas[y, x + 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y, x + 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y + 1, x - 1] < menorEtiqueta && Etiquetas[y + 1, x - 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y + 1, x - 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y + 1, x] < menorEtiqueta && Etiquetas[y + 1, x] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y + 1, x];
                                    alteracao = true;
                                }
                                if (Etiquetas[y + 1, x + 1] < menorEtiqueta && Etiquetas[y + 1, x + 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y + 1, x + 1];
                                    alteracao = true;
                                }
                                Etiquetas[y, x] = menorEtiqueta;
                            }
                        }
                    }

                    if (!alteracao)
                        break;

                    alteracao = false;

                    // Passagem 2: baixo para cima, direita para esquerda
                    for (int y = height - 2; y >= 0; y--)
                    {
                        for (int x = width - 2; x >= 0; x--)
                        {
                            if (Etiquetas[y, x] != 0)
                            {
                                int menorEtiqueta = Etiquetas[y, x];

                                // Checar vizinhos com validação de limites
                                if (Etiquetas[y - 1, x - 1] < menorEtiqueta && Etiquetas[y - 1, x - 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y - 1, x - 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y - 1, x] < menorEtiqueta && Etiquetas[y - 1, x] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y - 1, x];
                                    alteracao = true;
                                }
                                if (Etiquetas[y - 1, x + 1] < menorEtiqueta && Etiquetas[y - 1, x + 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y - 1, x + 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y, x - 1] < menorEtiqueta && Etiquetas[y, x - 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y, x - 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y, x + 1] < menorEtiqueta && Etiquetas[y, x + 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y, x + 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y + 1, x - 1] < menorEtiqueta && Etiquetas[y + 1, x - 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y + 1, x - 1];
                                    alteracao = true;
                                }
                                if (Etiquetas[y + 1, x] < menorEtiqueta && Etiquetas[y + 1, x] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y + 1, x];
                                    alteracao = true;
                                }
                                if (Etiquetas[y + 1, x + 1] < menorEtiqueta && Etiquetas[y + 1, x + 1] != 0)
                                {
                                    menorEtiqueta = Etiquetas[y + 1, x + 1];
                                    alteracao = true;
                                }
                                Etiquetas[y, x] = menorEtiqueta;
                            }
                        }
                    }
                } while (alteracao == true);

                return Etiquetas;
            }
        }


        public static int[] Corte(Image<Bgr, byte> img, int[,] imge, int c)
        {
            unsafe
            {
                int width = img.Width;
                int height = img.Height;

                // Inicialização das variáveis para os limites do corte
                int[] ret = new int[8];
                ret[0] = 0;
                ret[1] = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (imge[y, x] == c)
                        {
                            ret[0] = x;
                            ret[1] = y;
                            break;
                        }

                    }
                }

                ret[2] = width - 1;
                ret[3] = height - 1;
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        if (imge[y, x] == c)
                        {
                            ret[2] = x;
                            ret[3] = y;
                            break;
                        }

                    }
                }

                ret[4] = 0;
                ret[5] = 0;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (imge[y, x] == c)
                        {
                            ret[4] = x;
                            ret[5] = y;
                            break;
                        }

                    }
                }

                ret[6] = width - 1;
                ret[7] = height - 1;

                for (int x = width - 1; x >= 0; x--)
                {
                    for (int y = height - 1; y >= 0; y--)
                    {
                        if (imge[y, x] == c)
                        {
                            ret[6] = x;
                            ret[7] = y;
                            break;
                        }

                    }
                }
                return ret;

            }
        }












        public static double CompararImagensPixelPorPixel(Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            unsafe
            {
                MIplImage m = img1.MIplImage;
                MIplImage m2 = img2.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer();

                int totalPixels = img1.Width * img1.Height;
                int pixelsIguais = 0;

                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                // Garantir que as imagens possuem o mesmo tamanho
                if (img1.Width != img2.Width || img1.Height != img2.Height)
                {
                    return 0; // Sem correspondência
                }

                // Comparar pixel por pixel
                for (int y = 0; y < img1.Height; y++)
                {
                    for (int x = 0; x < img1.Width; x++)
                    {
                        int blue1 = (dataPtr + nChan * x + widthStep * y)[0];
                        int green1 = (dataPtr + nChan * x + widthStep * y)[1];
                        int red1 = (dataPtr + nChan * x + widthStep * y)[2];
                        int blue2 = (dataPtr2 + nChan * x + widthStep * y)[0];
                        int green2 = (dataPtr2 + nChan * x + widthStep * y)[1];
                        int red2 = (dataPtr2 + nChan * x + widthStep * y)[2];

                        if (blue1 == blue2 && green1 == green2 && red1 == red2)
                        {
                            pixelsIguais++;
                        }
                    }
                }

                // Calcular a porcentagem de pixels iguais
                return (double)pixelsIguais / totalPixels;
            }
        }








        public static void BinarizarCartasVermelhas(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.Convert<Hsv, byte>().MIplImage; // Converter para HSV
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Índice do pixel atual
                        byte* pixelPtr = dataPtrImg + x * nChan + y * widthStep;

                        // Obter componentes HSV
                        byte hue = pixelPtr[0];
                        byte saturation = pixelPtr[1];
                        byte value = pixelPtr[2];

                        // Condição para cor vermelha
                        if ((hue < 30 || hue > 340) && saturation > 0.5)
                        {
                            pixelPtr[0] = 255;   // H (opcional)
                            pixelPtr[1] = 255;   // S
                            pixelPtr[2] = 255; // V (Branco)
                        }
                        else
                        {
                            pixelPtr[0] = 0;
                            pixelPtr[1] = 0;
                            pixelPtr[2] = 0;
                        }
                    }
                }
            }
        }




        public static void BinarizarCartasPretas(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.Convert<Hsv, byte>().MIplImage; // Converter para HSV
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Índice do pixel atual
                        byte* pixelPtr = dataPtrImg + x * nChan + y * widthStep;

                        // Obter componentes HSV
                        byte hue = pixelPtr[0];
                        byte saturation = pixelPtr[1];
                        byte value = pixelPtr[2];

                        // Condição para cor preta
                        if (value < 0.5) // Brilho e saturação baixos
                        {
                            pixelPtr[0] = 255; // H
                            pixelPtr[1] = 255; // S
                            pixelPtr[2] = 255; // V
                        }
                        else
                        {
                            pixelPtr[0] = 0;   // H (opcional, pois não afeta binarização final)
                            pixelPtr[1] = 0;   // S
                            pixelPtr[2] = 0; // V (Branco)
                        }
                    }
                }
            }
        }



        public static int ClassificarEBinarizarCarta(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int pixelPretoCount = 0; // Variável para contar os pixels pretos

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Obter ponteiro para o pixel atual
                        byte* pixelPtr = dataPtr + y * widthStep + x * nChan;

                        // Obter valores RGB
                        byte blue = pixelPtr[0];
                        byte green = pixelPtr[1];
                        byte red = pixelPtr[2];


                        if (blue == 0 && green == 0 && red == 0)
                        {
                            pixelPretoCount++;
                        }

                    }
                }
                return pixelPretoCount;
            }


        }




        public static void BinarizarCartaComClassificacao(Image<Bgr, byte> imagem)
        {
            int inc = ClassificarEBinarizarCarta(imagem);
            if (inc > 20)
            {
                ImageClass.BinarizarCartasPretas(imagem);
            }
            else
            {
                ImageClass.BinarizarCartasVermelhas(imagem);
            }
        }




        public static string IdentificarCarta(Image<Bgr, byte> img)
        {
            string caminhoBaseDeDados = "C:\\Users\\HP\\Downloads\\BD\\BD";

            string[] Base_Dados = Directory.GetFiles(caminhoBaseDeDados, "*.jpg");

            double[] relacoes = new double[Base_Dados.Length];

            // Redimensionar e binarizar a imagem de entrada
            img = img.Resize(50, 50, INTER.CV_INTER_CUBIC);
            BinarizarCartaComClassificacao(img);

            // Processar a base de dados
            for (int i = 0; i < Base_Dados.Length; i++)
            {
                Image<Bgr, byte> imgBD = new Image<Bgr, byte>(Base_Dados[i]);
                imgBD = imgBD.Resize(50, 50, INTER.CV_INTER_CUBIC);
                BinarizarCartaComClassificacao(imgBD);

                // Comparar imagens
                relacoes[i] = ImageClass.CompararImagensPixelPorPixel(img, imgBD);
            }

            // Encontrar a melhor correspondência
            double maxRelacao = relacoes.Max();
            int index = Array.IndexOf(relacoes, maxRelacao);

            // Retornar o nome do arquivo correspondente
            string result = Path.GetFileNameWithoutExtension(Base_Dados[index]);
            return result;
        }


        public static void CorrigirRotacaoCarta(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int x_l, int y_b, int x_r, int y_t)
        {
            // 1. Calcular dx e dy (diferenças horizontais e verticais)
            float dx = Math.Abs(x_r - x_l);
            float dy = Math.Abs(y_b - y_t);

            // 2. Calcular o ângulo atual θ em graus
            float theta = (float)(Math.Atan2(dy, dx) * (180.0 / Math.PI));

            // 3. Diferença com 45 graus
            float diferenca = Math.Abs(45.0f - theta);

            // 4. Verificar se precisa de rotação
            if (diferenca > 1.0f) // Só roda se a diferença for maior que 1 grau
            {
                float anguloRotacao = diferenca; // Rotaciona pela diferença calculada
                if (theta > 45.0f) anguloRotacao = -diferenca; // Ajuste no sentido da rotação, se necessário

                // 5. Chamar a função de rotação
                Rotation(img, imgCopy, anguloRotacao);
            }
        }



        public static double EncontrarAngulo(Carta carta)
        {
            // Coordenadas do vetor entre o canto superior esquerdo e o superior direito
            int deltaX = carta.XSuperiorDireito - carta.XSuperiorEsquerdo;
            int deltaY = carta.YSuperiorDireito - carta.YSuperiorEsquerdo;

            // Calcular o ângulo em radianos
            double anguloRad = Math.Atan2(deltaY, deltaX);

            // Converter o ângulo para graus
            double anguloGraus = anguloRad * (180.0 / Math.PI);

            // Normalizar o ângulo para o intervalo [0, 180]
            if (anguloGraus < 0)
            {
                anguloGraus += 180;
            }

            return anguloGraus;
        }









    }
}







