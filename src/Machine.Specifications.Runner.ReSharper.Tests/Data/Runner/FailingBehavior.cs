using Machine.Specifications;

namespace Exploration
{
    public interface IVehicle
    {
        bool IsEngineRunning { get; }
    }

    public class Car : IVehicle
    {
        public bool IsEngineRunning
        {
            get { return true; }
        }

        public void StartEngine()
        {
        }
    }

    [Behaviors]
    public class AVehicleThatHasBeenStarted
    {
        protected static IVehicle Subject;

        It should_have_a_running_engine = () =>
            Subject.IsEngineRunning.ShouldBeFalse();

        It should_not_be_null = () =>
            Subject.ShouldNotBeNull();
    }

    [Subject("Starting")]
    class When_the_car_is_started
    {
        protected static Car Subject;

        Establish context = () =>
            Subject = new Car();

        Because of = () =>
            Subject.StartEngine();

        Behaves_like<AVehicleThatHasBeenStarted> a_vehicle_that_has_been_started;
    }
}
