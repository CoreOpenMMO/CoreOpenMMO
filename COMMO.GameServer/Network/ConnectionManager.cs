using System.Collections.Concurrent;
using System.Collections.Generic;
using COMMO.Domain.Interfaces.Services;
using COMMO.Network;

namespace COMMO.GameServer.Network {
    public sealed class ConnectionManager {
        private readonly IPlayerService _playerService;
        private readonly IAccountService _accountService;
        private readonly ConcurrentQueue<PendingRequest> _uhconnections;

        public ConnectionManager(IPlayerService playerService, IAccountService accountService) {
            _playerService = playerService;
            _accountService = accountService;
            _uhconnections = new ConcurrentQueue<PendingRequest>();
        }
        
        public void AddConnection(PendingRequest uhconnection) => _uhconnections.Enqueue(uhconnection);
        public bool PullConnection(out PendingRequest outUhConnection) => _uhconnections.TryDequeue(out outUhConnection);
    }
}
