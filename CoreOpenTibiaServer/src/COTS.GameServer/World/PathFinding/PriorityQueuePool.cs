using System;
using System.Collections.Concurrent;

namespace COTS.GameServer.World.PathFinding {
	public sealed class PriorityQueuePool {
		private const int InitialPoolSize = 256;
		private const int MaximumPoolSize = 2048;

		private readonly BlockingCollection<PriorityQueue> _pool;

		private PriorityQueuePool() {
			_pool = new BlockingCollection<PriorityQueue>();
			for (var i = 0; i < 256; i++)
				_pool.Add(new PriorityQueue(AStarAlgorithm.MaximumNodes));
		}

		public static readonly PriorityQueuePool Instance = new PriorityQueuePool();

		public PriorityQueue RentPriorityQueue() {
			if (_pool.TryTake(out var queue))
				return queue;
			else
				return new PriorityQueue(AStarAlgorithm.MaximumNodes);
		}

		public void ReturnPriorityQueue(PriorityQueue queue) {
			if (queue == null)
				throw new ArgumentNullException(nameof(queue));

			if (queue.Count < MaximumPoolSize)
				_pool.Add(queue);
		}
	}
}
