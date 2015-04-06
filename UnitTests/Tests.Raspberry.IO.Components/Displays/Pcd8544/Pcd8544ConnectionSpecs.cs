using System.Diagnostics;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using FluentAssertions;
using NUnit.Framework;
using Raspberry.IO;
using Raspberry.IO.Components.Displays.Pcd8544;
using Raspberry.IO.SerialPeripheralInterface;

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace Tests.Raspberry.IO.Components.Displays.Pcd8544.Pcd8544ConnectionSpecs
{
    public abstract class Pcd8544ConnectionSpec : Spec {
        protected readonly INativeSpiConnection spiConnection = A.Fake<INativeSpiConnection>();
        protected readonly IOutputBinaryPin resetPin = A.Fake<IOutputBinaryPin>();
        protected readonly IOutputBinaryPin dcModePin = A.Fake<IOutputBinaryPin>();
        protected readonly Pcd8544Connection sut;

        protected Pcd8544ConnectionSpec() {
            sut = CreateConnection();
        }

        protected Pcd8544Connection CreateConnection() {
            return new Pcd8544Connection(spiConnection, resetPin, dcModePin);
        }
    }

    [TestFixture]
    public class If_a_connection_instance_has_been_created : Pcd8544ConnectionSpec
    {
        private readonly Stopwatch timer = new Stopwatch();

        protected override void EstablishContext() {
            resetPin
                .CallsTo(pin => pin.Write(false))
                .Invokes(_ => timer.Start());

            resetPin
                .CallsTo(pin => pin.Write(true))
                .Invokes(_ => timer.Stop());
        }

        [Test]
        public void Shall_the_reset_signal_being_set_to_LOW_for_at_least_50ms_in_the_correct_order() {
            using (var scope = Fake.CreateScope()) {
                CreateConnection();

                using (scope.OrderedAssertions()) {
                    resetPin
                        .CallsTo(pin => pin.Write(false))
                        .MustHaveHappened(Repeated.Exactly.Once);

                    resetPin
                        .CallsTo(pin => pin.Write(true))
                        .MustHaveHappened(Repeated.Exactly.Once);
                }
            }

            timer.ElapsedMilliseconds
                .Should()
                .BeGreaterOrEqualTo(50);
        }
    }
}