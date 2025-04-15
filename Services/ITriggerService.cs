namespace BackgroundJobCodingChallenge.Services;

/// Meant for use with actions to be performed on a schedule or with a manual trigger
/// Most likely for use with the user csv upload and finance data sync
public interface ITriggerService
{
    delegate void FUnsubscribe();

    delegate Task FExecuteAsync(CancellationToken cancellationToken);

    FUnsubscribe Subscribe(FExecuteAsync executeAsync);

    void Unsubscribe(FExecuteAsync executeAsync);
}
