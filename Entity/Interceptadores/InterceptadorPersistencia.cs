using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Entity.Interceptadores
{
    public class InterceptadorPersistencia : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData, 
            InterceptionResult<int> result)
        {
            System.Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);

            return result;
        }
    }
}