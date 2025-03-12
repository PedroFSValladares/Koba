using Newtonsoft.Json;

namespace Discord.Net.Socket
{
    /// <summary>
    /// PayLoad com informções do evento recebido/enviado
    /// </summary>
    public class GatewayEvent
    {

        /// <summary>
        /// Código identificador do evento.
        /// </summary>
        public GatewayPayloadOpCode op { get; set; }

        /// <summary>
        /// Dados do Evento.
        /// </summary>
        public dynamic d { get; set; }

        /// <summary>
        /// Numero a ser utilizado nos eventos <see cref="GatewayPayloadOpCode.Resume"/> e 
        /// <see cref="GatewayPayloadOpCode.HeartBeat"/>
        /// </summary>
        public int? s { get; set; }

        /// <summary>
        /// Nome do evento
        /// </summary>
        public string? t { get; set; }
    }
}
