namespace LokiLoggingProvider.UnitTests.Logger
{
    using System;
    using System.Collections.Generic;
    using LokiLoggingProvider.Logger;
    using Xunit;

    public class NullExternalScopeProviderUnitTests
    {
        [Fact]
        public void When_CallingForEachScope_Expect_NothingDone()
        {
            // Arrange
            NullExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

            static void Callback(object scope, List<object> state) => state.Add(scope);
            List<object> scopes = new List<object>();

            // Act
            scopeProvider.ForEachScope(Callback, scopes);

            // Assert
            Assert.Empty(scopes);
        }

        [Fact]
        public void When_PushingAndDisposingScope_Expect_NoExceptions()
        {
            // Arrange
            NullExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

            // Act
            Exception result = Record.Exception(() =>
            {
                IDisposable scope = scopeProvider.Push("My Scope");
                scope.Dispose();
            });

            // Assert
            Assert.Null(result);
        }
    }
}
