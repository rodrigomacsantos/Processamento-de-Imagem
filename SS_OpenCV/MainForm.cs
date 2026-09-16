using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CG_OpenCV
{ 
    public partial class MainForm : Form
    {
        Image<Bgr, Byte> img = null; // working image
        Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        string title_bak = "";

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();
                ImageViewer.Image = img.Bitmap;
                ImageViewer.Refresh();
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
                return; 
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Binarizacao(img);
            int[,] etiquetas = ImageClass.EtiquetasFuncao(img);
            List<int> list = ImageClass.Sorting(etiquetas);
            foreach (int x in list)
            {
                ImageClass.Corte(img, etiquetas, x);
            }
            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }


        private async void binorizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // Verificar se a imagem já foi aberta
                return;

            Cursor = Cursors.WaitCursor; // Alterar para cursor de espera

            // Copiar a imagem original
            imgUndo = img.Copy();

            // Processar a imagem e obter etiquetas
            ImageClass.Binarizacao(img);
            int[,] etiquetas = ImageClass.EtiquetasFuncao(img);

            // Obter lista de etiquetas únicas
            List<int> list = ImageClass.Sorting(etiquetas);
            textBox.Text = "Total de cartas: " + list.Count;

            foreach (int label in list) // Iterar sobre todas as etiquetas identificadas
            {
                // Obter as dimensões da região correspondente à etiqueta
                int[] dim = ImageClass.Corte(img, etiquetas, label);
                Rectangle carta = new System.Drawing.Rectangle(
                    Math.Min(dim[4], dim[6]),
                    Math.Min(dim[1], dim[3]),
                    Math.Abs(dim[4] - dim[6]),
                    Math.Abs(dim[3] - dim[1])
                );

                try
                {
                    // Recortar a região da carta
                    Image<Bgr, byte> imgt = imgUndo.Copy(carta);

                    // Atualizar o nome da carta identificado
                    string nome = ImageClass.IdentificarCarta(imgt); // Usar a imagem recortada
                    textBox.Text = $"Carta: {nome}";

                    // Exibir a imagem no visualizador
                    ImageViewer.Image = imgt.Bitmap;
                    ImageViewer.Refresh(); // Garantir a atualização do visualizador

                    await Task.Delay(5000); // Esperar 5 segundos antes de exibir a próxima carta
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar a etiqueta {label}: {ex.Message}");
                }
            }

            Cursor = Cursors.Default; // Restaurar cursor normal
        }












    }




}