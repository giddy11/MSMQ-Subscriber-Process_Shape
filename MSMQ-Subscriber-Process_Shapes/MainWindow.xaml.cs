using MSMQ.Messaging;
using MSMQ_Subscriber_Process_Shapes.Models;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MSMQ_Subscriber_Process_Shapes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Subscriber();
        }

        public void Subscriber()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!MessageQueue.Exists(@".\private$\MSMQ_MessagingApp"))
                {
                    return;
                }
                MessageQueue queue = new MessageQueue(@".\private$\MSMQ_MessagingApp");
                queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
                queue.BeginReceive();
            });
        }

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageQueue queue = (MessageQueue)sender;
                queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                Message message = queue.EndReceive(e.AsyncResult);

                var shapeData = JsonConvert.DeserializeObject<ShapeData>((string)message.Body);
                if (shapeData == null) return;
                Shape newShape = CreateShape(shapeData);
                if (newShape == null) return;

                // Set the position of the newshape on the canvas
                Canvas.SetLeft(newShape, shapeData.X);
                Canvas.SetTop(newShape, shapeData.Y);

                //Canvas.Children.Add(newShape);
                canvas.Children.Add(newShape);


                queue.BeginReceive();

            });
        }

        private Shape CreateShape(ShapeData shapeData)
        {
            Shape? newShape = Activator.CreateInstance(shapeData.ShapeType) as Shape;
            if (newShape == null) return null;

            newShape.Width = shapeData.Width;
            newShape.Height = shapeData.Height;
            newShape.Stroke = shapeData.Stroke;
            newShape.Fill = shapeData.Fill;

            //var size = new Size(newShape.Width, newShape.Height);
            ////var size = new Size(shape.Width, shape.Height);

            //newShape.Measure(size);
            //newShape.Arrange(new Rect(size));
            //newShape.UpdateLayout();
            return newShape;
        }

        /*private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var maxposition = e.GetPosition(canvas);
            var selectedShape = ToolBox.SelectedItem as Shape;

            // Check if the selected shape is null or the type is supported   
            if (selectedShape == null || !selectedShape.GetType().IsSubclassOf(typeof(Shape))) return;

            //foreach (var child in Canvas.Children.OfType<Shape>().ToList())
            //{
            //    Canvas.Children.Remove(child);
            //}
            var shape = Activator.CreateInstance(selectedShape.GetType()) as Shape;
            if (shape == null) return;

            // Set the default properties of the new shape            
            shape.Width = selectedShape.Width;
            shape.Height = selectedShape.Height;
            shape.Stroke = selectedShape.Stroke;
            shape.Fill = selectedShape.Fill;
            var size = new Size(selectedShape.Width, selectedShape.Height);
            //var size = new Size(shape.Width, shape.Height);

            shape.Measure(size);
            shape.Arrange(new Rect(size));
            shape.UpdateLayout();

            // Set the position of the new shape on the canvas
            Canvas.SetLeft(shape, maxposition.X);
            Canvas.SetTop(shape, maxposition.Y);

            // Add the new shape to the canvas
            canvas.Children.Add(shape);
        }*/
    }
}
