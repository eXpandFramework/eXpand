﻿using System;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.ClientServer{
    public class XpandSecuredDataServer : SecuredDataServer {
        public XpandSecuredDataServer(IServerSecurity serverSecurity, ISecuredSerializableObjectLayer securedSerializableObjectLayer)
            : base(serverSecurity, securedSerializableObjectLayer) {
        }

        public XpandSecuredDataServer(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler querySecurityEnvironmentHandler, ILogger logger, EventHandler<DataServiceOperationEventArgs> committingDelegate, bool allowICommandChannelDoWithSecurityContext)
            : base(dataLayer, querySecurityEnvironmentHandler, logger, committingDelegate, allowICommandChannelDoWithSecurityContext) {
        }

        public XpandSecuredDataServer(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler querySecurityEnvironmentHandler, ILogger logger)
            : base(dataLayer, querySecurityEnvironmentHandler, logger) {
        }

        public XpandSecuredDataServer(IDataLayer dataLayer, QueryRequestSecurityStrategyHandler querySecurityEnvironmentHandler)
            : base(dataLayer, querySecurityEnvironmentHandler) {
        }

        public XpandSecuredDataServer(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler securityEnvironmentProvider, ILogger logger, EventHandler<DataServiceOperationEventArgs> committingDelegate)
            : base(connectionString, dictionary, securityEnvironmentProvider, logger, committingDelegate) {
            _connectionstring = connectionString;
        }


        public XpandSecuredDataServer(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler securityEnvironmentProvider, ILogger logger)
            : base(connectionString, dictionary, securityEnvironmentProvider, logger) {
            _connectionstring = connectionString;
        }

        public XpandSecuredDataServer(string connectionString, XPDictionary dictionary, QueryRequestSecurityStrategyHandler securityEnvironmentProvider)
            : base(connectionString, dictionary, securityEnvironmentProvider) {
            _connectionstring = connectionString;
        }

        private readonly string _connectionstring;

        protected override ISecuredSerializableObjectLayer CreateDefaultSecuredSerializableObjectLayer(IDataLayer dataLayer,
        RequestSecurityStrategyProvider securityStrategyProvider, EventHandler<DataServiceOperationEventArgs> committingDelegate,
        bool allowICommandChannelDoWithSecurityContext) {
            string connectionString = String.IsNullOrEmpty(_connectionstring) ? ((ConnectionProviderSql)((BaseDataLayer)dataLayer).ConnectionProvider).ConnectionString : _connectionstring;

            var threadSafeDataLayer = new ThreadSafeDataLayer(dataLayer.Dictionary, new MultiDataStoreProxy(connectionString));
            return new SecuredSerializableObjectLayer(threadSafeDataLayer, securityStrategyProvider, allowICommandChannelDoWithSecurityContext);
        }
    }
}
