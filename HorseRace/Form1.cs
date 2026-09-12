using System.Collections.Concurrent;

namespace HorseRace
{
    public partial class Form1 : Form
    {
        private readonly (string Name, Color Color)[] _horses = new[]
        {
            ("Korona", Color.Red),
            ("Princ",  Color.Indigo),
            ("Babel",  Color.DodgerBlue),
            ("Orion",  Color.Magenta),
            ("Klark",  Color.Gold),
            ("Suzuki", Color.Olive),
            ("Nobel",  Color.SaddleBrown),
            ("Kruz",   Color.ForestGreen),
        };

        private const int RaceLength = 100;   
        private readonly Random _seedRandom = new();

        private readonly Dictionary<string, ColoredProgressBar> _bars = new();
        private Button _runButton = null!;
        private ListBox _resultsList = null!;

        private ConcurrentQueue<string> _finishOrder = null!;
        private int _finishedCount;

        public Form1()
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            Text = "Form1";
            ClientSize = new Size(1017, 508);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = true;

            int top = 70;
            const int rowHeight = 48;
            const int labelX = 20;
            const int barX = 90;
            const int barWidth = 620;
            const int barHeight = 30;

            foreach (var (name, color) in _horses)
            {
                var label = new Label
                {
                    Text = name,
                    Location = new Point(labelX, top + 8),
                    AutoSize = true,
                    Font = new Font(Font.FontFamily, 10f)
                };
                Controls.Add(label);

                var bar = new ColoredProgressBar
                {
                    Location = new Point(barX, top),
                    Size = new Size(barWidth, barHeight),
                    Minimum = 0,
                    Maximum = RaceLength,
                    Value = 0,
                    BarColor = color
                };
                Controls.Add(bar);
                _bars[name] = bar;

                top += rowHeight;
            }

            _resultsList = new ListBox
            {
                Location = new Point(770, 70),
                Size = new Size(200, 380),
                Font = new Font(Font.FontFamily, 10f)
            };
            Controls.Add(_resultsList);

            _runButton = new Button
            {
                Text = "RUN !!!",
                Location = new Point(430, 460),
                Size = new Size(150, 34),
                Font = new Font(Font.FontFamily, 10f, FontStyle.Bold)
            };
            _runButton.Click += RunButton_Click;
            Controls.Add(_runButton);
        }


        private async void RunButton_Click(object? sender, EventArgs e)
        {
            _runButton.Enabled = false;
            _resultsList.Items.Clear();
            _finishOrder = new ConcurrentQueue<string>();
            _finishedCount = 0;

            foreach (var bar in _bars.Values)
                bar.Value = 0;

            var horseTasks = _horses.Select(h => Task.Run(() => RunHorse(h.Name))).ToArray();

            await Task.WhenAll(horseTasks);

            ShowResults();
            _runButton.Enabled = true;
        }

        private void RunHorse(string name)
        {
            var rng = new Random(Guid.NewGuid().GetHashCode());
            var bar = _bars[name];
            int progress = 0;

            while (progress < RaceLength)
            {
                int step = rng.Next(1, 6);          
                int delay = rng.Next(40, 160);       

                Thread.Sleep(delay);

                progress = Math.Min(progress + step, RaceLength);
                int progressSnapshot = progress;

                bar.Invoke(() => bar.Value = progressSnapshot);
            }

            _finishOrder.Enqueue(name);
            Interlocked.Increment(ref _finishedCount);
        }

        private void ShowResults()
        {
            int place = 1;
            foreach (var name in _finishOrder)
            {
                _resultsList.Items.Add($"{place}. {name}");
                place++;
            }
        }
    }
}