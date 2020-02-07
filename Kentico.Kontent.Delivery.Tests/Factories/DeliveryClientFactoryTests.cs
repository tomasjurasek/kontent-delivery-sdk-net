﻿using FakeItEasy;
using FluentAssertions;
using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kentico.Kontent.Delivery.Tests.Factories
{
    public class DeliveryClientFactoryTests
    {
        private readonly IOptions<DeliveryOptions> _deliveryOptionsMock;
        private readonly IOptionsMonitor<DeliveryClientFactoryOptions> _deliveryClientFactoryOptionsMock;
        private readonly IServiceProvider _serviceProviderMock;

        private const string _clientName = "ClientName";

        public DeliveryClientFactoryTests()
        {
            _deliveryOptionsMock = A.Fake<IOptions<DeliveryOptions>>();
            _deliveryClientFactoryOptionsMock = A.Fake<IOptionsMonitor<DeliveryClientFactoryOptions>>();
            _serviceProviderMock = A.Fake<IServiceProvider>();
        }
        [Fact]
        public void GetNamedClient_WithCorrectName_GetClient()
        {
            var deliveryClient = new DeliveryClient(_deliveryOptionsMock);
            var deliveryClientFactoryOptions = new DeliveryClientFactoryOptions();
            deliveryClientFactoryOptions.DeliveryClientsActions.Add(() => deliveryClient);

            A.CallTo(() => _deliveryClientFactoryOptionsMock.Get(_clientName))
                .Returns(deliveryClientFactoryOptions);

            var deliveryClientFactory = new Delivery.DeliveryClientFactory(_deliveryClientFactoryOptionsMock, _serviceProviderMock);

            var result = deliveryClientFactory.Get(_clientName);

            result.Should().Be(deliveryClient);
        }

        [Fact]
        public void GetNamedClient_WithWrongName_GetNull()
        {
            var deliveryClient = new DeliveryClient(_deliveryOptionsMock);
            var deliveryClientFactoryOptions = new DeliveryClientFactoryOptions();
            deliveryClientFactoryOptions.DeliveryClientsActions.Add(() => deliveryClient);

            A.CallTo(() => _deliveryClientFactoryOptionsMock.Get(_clientName))
                .Returns(deliveryClientFactoryOptions);

            var deliveryClientFactory = new Delivery.DeliveryClientFactory(_deliveryClientFactoryOptionsMock, _serviceProviderMock);

            var result = deliveryClientFactory.Get("WrongName");

            result.Should().BeNull();
        }
    }
}
