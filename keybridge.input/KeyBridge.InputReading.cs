using Newtonsoft.Json;
using SharpHook.Simulation;
using SharpHook.Data;


namespace KeyBridge.InputReading
{
    public class KeyPress
    {
        public bool isAltPressed = false;
        public bool isShiftPressed = false;
        public bool isControlPressed = false;

        public char KeyChar;



        public static KeyPress ReadKeyPress()
        {


            var key = Console.ReadKey(intercept: true);
            var result = new KeyPress
            {
                KeyChar = key.KeyChar,
                isAltPressed = key.Modifiers.HasFlag(ConsoleModifiers.Alt),
                isShiftPressed = key.Modifiers.HasFlag(ConsoleModifiers.Shift),
                isControlPressed = key.Modifiers.HasFlag(ConsoleModifiers.Control)
            };


            return result;
        }


        public static string ReadKeyPressJSON()
        {
            var keyPress = ReadKeyPress();

            return JsonConvert.SerializeObject(keyPress);
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static KeyPress? Parse(string input)
        {
            return JsonConvert.DeserializeObject<KeyPress>(input);
        }
    }
    public static class KeyPressConverter
    {
        public static void Simulate(KeyPress input, EventSimulator simulator)
        {
            // Press modifiers first
            if (input.isControlPressed)
                simulator.SimulateKeyPress(KeyCode.VcLeftControl);

            if (input.isAltPressed)
                simulator.SimulateKeyPress(KeyCode.VcLeftAlt);

            if (input.isShiftPressed)
                simulator.SimulateKeyPress(KeyCode.VcLeftShift);

            // Press and release the main key
            var keyCode = ToKeyCode(input.KeyChar);

            if (keyCode.HasValue)
            {
                simulator.SimulateKeyPress(keyCode.Value);
                simulator.SimulateKeyRelease(keyCode.Value);
            }

            // Release modifiers in reverse order
            if (input.isShiftPressed)
                simulator.SimulateKeyRelease(KeyCode.VcLeftShift);

            if (input.isAltPressed)
                simulator.SimulateKeyRelease(KeyCode.VcLeftAlt);

            if (input.isControlPressed)
                simulator.SimulateKeyRelease(KeyCode.VcLeftControl);
        }

        private static KeyCode? ToKeyCode(char c)
        {
            if (c >= 'a' && c <= 'z')
                return (KeyCode)((int)KeyCode.VcA + (c - 'a'));

            if (c >= 'A' && c <= 'Z')
                return (KeyCode)((int)KeyCode.VcA + (c - 'A'));

            if (c >= '0' && c <= '9')
                return (KeyCode)((int)KeyCode.Vc0 + (c - '0'));

            return c switch
            {
                ' ' => KeyCode.VcSpace,
                '\n' => KeyCode.VcEnter,
                '\r' => KeyCode.VcEnter,
                '\t' => KeyCode.VcTab,
                _ => null
            };
        }
    }

    public static class InputEmitter
    {
        public static void Emit(KeyPress key)
        {
            EventSimulator simulator = EventSimulator.Create("KeyBridge");
            try
            {
                
            KeyPressConverter.Simulate(key, simulator);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}