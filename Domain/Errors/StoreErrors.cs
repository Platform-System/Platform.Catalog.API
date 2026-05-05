using Platform.Domain.Common;

namespace Platform.Catalog.API.Domain.Errors
{
    public static class StoreErrors
    {
        public static Error AlreadyVerified => new("Store.AlreadyVerified", "Store is already verified.");
        public static Error VerificationNotRequested => new("Store.VerificationNotRequested", "Store has not requested verification.");
        public static Error CannotRequestVerification => new("Store.CannotRequestVerification", "Store cannot request verification in the current state.");
    }
}
