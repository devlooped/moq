using System;
using Xunit;

namespace Moq.Tests
{
	public class Demo
	{
		private static string TALISKER = "Talisker";

		[Fact]
		public void FillingRemovesInventoryIfInStock()
		{
			//setup - data
			var order = new Order(TALISKER, 50);
			var mock = new Mock<IWarehouse>();

			//setup - expectations
			mock.Setup(x => x.HasInventory(TALISKER, 50)).Returns(true);

			//exercise
			order.Fill(mock.Object);

			//verify state
			Assert.True(order.IsFilled);
			//verify interaction
			mock.VerifyAll();
		}

		public void FillingDoesNotRemoveIfNotEnoughInStock()
		{
			//setup - data
			var order = new Order(TALISKER, 50);
			var mock = new Mock<IWarehouse>();

			//setup - expectations
			mock.Setup(x => x.HasInventory(It.IsAny<string>(), It.IsInRange(0, 100, Range.Inclusive))).Returns(false);
			mock.Setup(x => x.Remove(It.IsAny<string>(), It.IsAny<int>())).Throws(new InvalidOperationException());

			//exercise
			order.Fill(mock.Object);

			//verify
			Assert.False(order.IsFilled);
		}

		public void TestPresenterSelection()
		{
			var mockView = new Mock<IOrdersView>();
			var presenter = new OrdersPresenter(mockView.Object);

			// Check that the presenter has no selection by default
			Assert.Null(presenter.SelectedOrder);

			// Finally raise the event with a specific arguments data
			mockView.Raise(mv => mv.OrderSelected += null, new OrderEventArgs { Order = new Order("moq", 500) });

			// Now the presenter reacted to the event, and we have a selected order
			Assert.NotNull(presenter.SelectedOrder);
			Assert.Equal("moq", presenter.SelectedOrder.ProductName);
		}

		public class OrderEventArgs : EventArgs
		{
			public Order Order { get; set; }
		}

		public interface IOrdersView
		{
			event EventHandler<OrderEventArgs> OrderSelected;
		}

		public class OrdersPresenter
		{
			public OrdersPresenter(IOrdersView view)
			{
				view.OrderSelected += (sender, args) => DoOrderSelection(args.Order);
			}

			public Order SelectedOrder { get; private set; }

			private void DoOrderSelection(Order selectedOrder)
			{
				// Do something when the view selects an order.
				SelectedOrder = selectedOrder;
			}
		}

		public interface IWarehouse
		{
			bool HasInventory(string productName, int quantity);
			void Remove(string productName, int quantity);
		}

		public class Order
		{
			public string ProductName { get; private set; }
			public int Quantity { get; private set; }
			public bool IsFilled { get; private set; }

			public Order(string productName, int quantity)
			{
				this.ProductName = productName;
				this.Quantity = quantity;
			}

			public void Fill(IWarehouse warehouse)
			{
				if (warehouse.HasInventory(ProductName, Quantity))
				{
					warehouse.Remove(ProductName, Quantity);
					IsFilled = true;
				}
			}

		}

		[Fact]
		[Obsolete]
		public void ProjectHomePageDemo()
		{
			var mock = new Mock<ILoveThisFramework>();

			// WOW! No record/reply weirdness?! :)
			mock.Setup(framework => framework.ShouldDownload(It.IsAny<Version>()))
				 .Callback((Version version) =>
					  Console.WriteLine("Someone wanted version {0}!!!", version))
				 .Returns(true)
				 .AtMostOnce();

			// Hand mock.Object as a collaborator and exercise it, 
			// like calling methods on it...
			ILoveThisFramework lovable = mock.Object;
			bool download = lovable.ShouldDownload(new Version("2.0.0.0"));

			mock.VerifyAll();
			mock.Verify(framework => framework.ShouldDownload(new Version("2.0.0.0")));
		}

		public interface ILoveThisFramework
		{
			bool ShouldDownload(Version version);
		}

		public void When_user_forgot_password_should_save_user()
		{
			var userRepository = new Mock<IUserRepository>();
			var smsSender = new Mock<ISmsSender>();

			var theUser = new User { HashedPassword = "this is not hashed password" };

			userRepository.Setup(x => x.GetUserByName("ayende")).Returns(theUser);

			var controllerUnderTest = new LoginController(userRepository.Object, smsSender.Object);

			controllerUnderTest.ForgotMyPassword("ayende");

			userRepository.Verify(x => x.Save(theUser));
		}

		public interface ISmsSender { }
		public interface IUserRepository
		{
			void Save(User user);
			User GetUserByName(string username);
		}

		public class User
		{
			public string HashedPassword { get; set; }
		}

		public class LoginController
		{
			public LoginController(IUserRepository repo, ISmsSender sender)
			{
			}

			public void ForgotMyPassword(string username) { }
		}

	}
}
