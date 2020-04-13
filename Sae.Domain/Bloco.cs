using Sae.Domain.Enum;

namespace Sae.Domain
{
    public class Bloco
    {
        public string Nome { get; }
        public float Largura { get; }
        public ushort Altura { get; }
        public ushort Comprimento { get; }
        public float AlturaParede { get; }
        public TipoBloco TipoBloco { get; }

        public Bloco(ushort altura, float largura, ushort comprimento, float alturaParede, TipoBloco tipoBloco)
        {
            Nome = tipoBloco.ToString();
            Altura = altura;
            Largura = largura;
            Comprimento = comprimento;
            AlturaParede = alturaParede;
            TipoBloco = tipoBloco;
        }
    }
}
