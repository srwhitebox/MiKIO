﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Mvc.Handlers
{
    public class MvcHandler : IUpstreamHandler
    {
        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            throw new NotImplementedException();
        }
    }

}
