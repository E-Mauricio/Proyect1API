using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest
{
    internal class DbSetWrapper<T> : DbSet<T>, IQueryable<T>, IDbAsyncEnumerable<T>
        where T : class
    {
        private readonly IQueryable<T> _data;

        public DbSetWrapper(IQueryable<T> data)
        {
            _data = data;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(_data.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        public Type ElementType => _data.ElementType;
        public Expression Expression => _data.Expression;
        public IQueryProvider Provider => new TestDbAsyncQueryProvider<T>(_data.Provider);
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _innerEnumerator;

        public TestDbAsyncEnumerator(IEnumerator<T> innerEnumerator)
        {
            _innerEnumerator = innerEnumerator;
        }

        public void Dispose()
        {
            _innerEnumerator.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_innerEnumerator.MoveNext());
        }

        public T Current => _innerEnumerator.Current;

        object IDbAsyncEnumerator.Current => Current;
    }

    internal class TestDbAsyncQueryProvider<T> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _innerProvider;

        internal TestDbAsyncQueryProvider(IQueryProvider innerProvider)
        {
            _innerProvider = innerProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _innerProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _innerProvider.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider => new TestDbAsyncQueryProvider<T>(this);
    }
}
