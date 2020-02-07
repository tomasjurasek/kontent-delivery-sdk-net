﻿using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kentico.Kontent.Delivery
{
    /// <summary>
    /// A factory class for <see cref="IDeliveryClient"/>
    /// </summary>
    public class DeliveryClientFactory  : IDeliveryClientFactory
    {
        private ConcurrentDictionary<string, IDeliveryClient> _cachedDeliveryClients = new ConcurrentDictionary<string, IDeliveryClient>();
        private readonly IOptionsMonitor<DeliveryClientFactoryOptions> _optionsMonitor;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryClientFactory"/> class
        /// </summary>
        /// <param name="optionsMonitor">A <see cref="DeliveryClientFactory"/> options</param>
        /// <param name="serviceProvider">A <see cref="IServiceProvider"/> instance</param>
        public DeliveryClientFactory(IOptionsMonitor<DeliveryClientFactoryOptions> optionsMonitor, IServiceProvider serviceProvider)
        {
            _optionsMonitor = optionsMonitor;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a named <see cref="IDeliveryClient"/>.
        /// </summary>
        /// <param name="name">A name of <see cref="IDeliveryClient"/> configuration</param>
        /// <returns>The <see cref="IDeliveryClient"/> instance that represents named client</returns>
        public IDeliveryClient Get(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!_cachedDeliveryClients.TryGetValue(name, out var client))
            {
                var options = _optionsMonitor.Get(name);
                client = options.DeliveryClientsActions.FirstOrDefault()?.Invoke();
                _cachedDeliveryClients.TryAdd(name, client);
            }

            return client;
        }

        /// <summary>
        /// Returns an <see cref="IDeliveryClient"/>.
        /// </summary>
        /// <returns>The <see cref="IDeliveryClient"/> instance that represents client</returns>
        public IDeliveryClient Get()
        {
            return _serviceProvider.GetRequiredService<IDeliveryClient>();
        }
    }
}
