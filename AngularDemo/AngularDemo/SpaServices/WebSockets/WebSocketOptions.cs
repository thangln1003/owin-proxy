using System;
using System.Collections.Generic;

namespace AngularDemo.SpaServices.WebSockets
{
    /// <summary>Configuration options for the WebSocketMiddleware</summary>
    public class WebSocketOptions
    {
        public WebSocketOptions()
        {
            this.KeepAliveInterval = TimeSpan.FromMinutes(2.0);
            this.ReceiveBufferSize = 4096;
            this.AllowedOrigins = (IList<string>)new List<string>();
        }

        /// <summary>
        /// Gets or sets the frequency at which to send Ping/Pong keep-alive control frames.
        /// The default is two minutes.
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; }

        /// <summary>
        /// Gets or sets the size of the protocol buffer used to receive and parse frames.
        /// The default is 4kb.
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Set the Origin header values allowed for WebSocket requests to prevent Cross-Site WebSocket Hijacking.
        /// By default all Origins are allowed.
        /// </summary>
        public IList<string> AllowedOrigins { get; }
    }
}