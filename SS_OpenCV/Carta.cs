namespace CG_OpenCV
{
    public class Carta
    {
        // Etiqueta ou identificador único da carta
        public int Etiqueta { get; set; }

        // Coordenadas do canto superior esquerdo
        public int XSuperiorEsquerdo { get; set; }
        public int YSuperiorEsquerdo { get; set; }

        // Coordenadas do canto superior direito
        public int XSuperiorDireito { get; set; }
        public int YSuperiorDireito { get; set; }

        // Coordenadas do canto inferior esquerdo
        public int XInferiorEsquerdo { get; set; }
        public int YInferiorEsquerdo { get; set; }

        // Coordenadas do canto inferior direito
        public int XInferiorDireito { get; set; }
        public int YInferiorDireito { get; set; }

        // Construtor para inicializar os valores
        public Carta(int etiqueta)
        {
            Etiqueta = etiqueta;
        }

        // Método opcional para exibir os valores
        public override string ToString()
        {
            return $"Carta {Etiqueta}:\n" +
                   $"  Superior Esquerdo: ({XSuperiorEsquerdo}, {YSuperiorEsquerdo})\n" +
                   $"  Superior Direito: ({XSuperiorDireito}, {YSuperiorDireito})\n" +
                   $"  Inferior Esquerdo: ({XInferiorEsquerdo}, {YInferiorEsquerdo})\n" +
                   $"  Inferior Direito: ({XInferiorDireito}, {YInferiorDireito})";
        }
    }
}