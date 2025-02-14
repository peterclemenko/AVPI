﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Speech.Synthesis;
using InputManager;

/*
 * A single action is composed of 
 * name - the name identifying the action
 * sequence of Actions
 * Actions are carried out sequentially one at time
 */

namespace GAVPI
{
    public class VI_Action_Sequence : VI_TriggerEvent
    {
        // These type lists are used to populate ui elements,
        // their (array string) value must match the class name specified bellow, ex : Action : Data_Set
        // this is particularly important since the string will be cast to a class instance later.

        public static List<string> PressAction_Types = new List<string>(
            new string[] { 
                "KeyDown", "KeyUp", "KeyPress",
                "MouseKeyDown","MouseKeyUp","MouseKeyPress"
            });
        public static List<string> SpeechAction_Types = new List<string>(
            new string[] { 
               "Speak",
               "Data_Speak"
            });
        public static List<string> PlaySoundAction_Types = new List<string>(
            new string[] {
                "Play_Sound"
            });
        public static List<string> TimingAction_Types = new List<string>(
            new string[] { 
               "Wait"
            });
        public static List<string> DataAction_Types = new List<string>(
            new string[] { 
               "Data_Set","Data_Decrement","Data_Increment"
            });
              

        public string name {get; set;}
        public string type { get; set; }
        public string comment { get; set; }

        //sequences do not use value, but must have it as part of TriggerEvent interface
        public string value { get; set; }

        public List<Action> action_sequence;
        
        public VI_Action_Sequence()
        {
            action_sequence = new List<Action>();
            this.name  = "new sequence";
            this.type  = this.GetType().Name;
            this.value = null;
            this.comment = "";
        }

        public VI_Action_Sequence(string name)
        {
            action_sequence = new List<Action>();
            this.name = name;
            this.type = this.GetType().Name;
            this.value = null;
            this.comment = "";
        }
        public List<Action> get_Action_sequence()
        {
            return this.action_sequence;
        }
        public void set_Action_sequence(List<Action> new_Action_sequence)
        {
            this.action_sequence = new_Action_sequence;
        }

        public void Add(Action new_Action)
        {
            action_sequence.Add(new_Action);
        }
        public void Remove(Action rm_Action)
        {
            action_sequence.Remove(rm_Action);
        }
        public void run()
        {
            foreach (Action action in action_sequence)
            {
                action.run();
                // A little hotfix for starcitizen, it cant handle going fast.
                Thread.Sleep(20);
            }
        }
    }

     /*
      *  Arbitrary abstract Action
      *  Currently one of 
      *  Keyboard up/down/press
      *  Mouse up/down/press
      *  Wait 
      *  ...add your own
      *  ie: public partial class PlaySound() ...
     */
    public abstract class Action
    {
        public string type { get; set; }
        public abstract string value { get; set; }

        public abstract void run();

        public Action()
        {
            this.type = this.GetType().Name;
        }
        public Action(string value)
        {
            this.value = value;
            this.type = this.GetType().Name;
        }
    }
    public partial class KeyDown : Action
    {
        public KeyDown(string value) : base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;set;
        }
        public override void run()
        {
            Keyboard.KeyDown((Keys)Enum.Parse(typeof(Keys), value));
        }
    }
    public partial class KeyUp : Action
    {
        public KeyUp(string value): base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            Keyboard.KeyUp((Keys)Enum.Parse(typeof(Keys), value));
        }
    }
    public partial class KeyPress : Action
    {
        public KeyPress(string value): base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            Keyboard.KeyPress((Keys)Enum.Parse(typeof(Keys), value));
        }
    }

    public partial class MouseKeyDown : Action
    {
        public MouseKeyDown(string value): base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            Mouse.ButtonDown((Mouse.MouseKeys)Enum.Parse(typeof(Mouse.MouseKeys), value));
        }
    }
    public partial class MouseKeyUp : Action
    {
        public MouseKeyUp(string value) : base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            Mouse.ButtonUp((Mouse.MouseKeys)Enum.Parse(typeof(Mouse.MouseKeys), value));
        }
    }
    public partial class MouseKeyPress : Action
    {
        public MouseKeyPress(string value) : base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            Mouse.PressButton((Mouse.MouseKeys)Enum.Parse(typeof(Mouse.MouseKeys), value));
        }
    }
    public partial class Wait : Action
    {
        public Wait(string value) : base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            //Thread.Sleep( Int32.Parse(value) ); // doesnt like this, why?
            int sleep = Int32.Parse(value);
            Thread.Sleep(sleep);
        }
    }
    public partial class Speak : Action
    {
        SpeechSynthesizer Profile_Synthesis;
        public Speak(SpeechSynthesizer Profile_Synthesis, string value) : base(value)
        {
            this.Profile_Synthesis = Profile_Synthesis;
            this.value = value;
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            try
            {
                Profile_Synthesis.SpeakAsync(value);
            }
            catch(Exception synth_err)
            {
                MessageBox.Show("Speech Profile_Synthesis Err : "+synth_err.Message,"Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
            }
        }
    }
    #region PlaySound Actions
    // More formats support
    public partial class Play_Sound : Action
    {
        WMPLib.WindowsMediaPlayer player;
        public Play_Sound(string filename)
        {
            // a common way of doing this, thanks stackoverflow.
            this.value = filename;
            try
            {
                player = new WMPLib.WindowsMediaPlayer();
            }
            catch (Exception e)
            {
                // TODO : Notify error.
            }
        }
        public override string value
        {
            get;
            set;
        }
        public override void run()
        {
            try
            {
                player.URL = this.value;
                player.controls.play();
            }
            catch (Exception e)
            {
                // TODO : Notify error.
            }
        }
    }
    #endregion

    #region Data Actions
    public partial class Data_Decrement : Action 
    {
        VI_Data data;
        public Data_Decrement( VI_Data data, string value) : base(value) 
        {
            this.value = value;
        }
        public override string value
        {
            get { return data.value.ToString(); }
            set { this.value = value; }
        }
        public override void run()
        {
            
        }
    }
    public partial class Data_Increment : Action
    {
        VI_Data data;
        public Data_Increment(VI_Data data, string value)
            : base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get { return data.value.ToString(); }
            set { this.value = value; }
        }
        public override void run()
        {

        }
    }
    public partial class Data_Set : Action
    {
        VI_Data data;
        public Data_Set(VI_Data data, string value)
            : base(value)
        {
            this.value = value;
        }
        public override string value
        {
            get { return data.value.ToString(); }
            set { this.value = value; }
        }
        public override void run()
        {


        }
    }
    public partial class Data_Speak : Action
    {
        /* Data Speak's value is the data elements key (it's name)
         * however on invocation it will query data.value for the actual
         * value. (in other words we access the value by reference)
         * Robert (04.12.15)
         */
        SpeechSynthesizer Profile_Synthesis;
        VI_Data data;

        public Data_Speak(SpeechSynthesizer Profile_Synthesis, VI_Data data)
        {
            this.Profile_Synthesis = Profile_Synthesis;
            this.data  = data;
        }

        public override string value
        {
            get { return data.name; }
            set { this.value = data.name; }
        }

        //public ValueType 
        public override void run()
        {
            try
            {
                Profile_Synthesis.SpeakAsync(data.value.ToString());
            }
            catch (Exception synth_err)
            {
                MessageBox.Show("Speech Profile_Synthesis Err : " + synth_err.Message, "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
            }


        }
    }

    // Simple WAV implementation taken from AVPI
    //public partial class Play_WAV : Action
    //{
    //    System.Media.SoundPlayer wav;

    //    public Play_WAV(string filename)
    //    {
    //        this.value = filename;
    //        try
    //        {
    //            wav = new System.Media.SoundPlayer(this.value);
    //        }
    //        catch (Exception e)
    //        {
    //            // TODO : Notify error.
    //        }
    //    }

    //    public override void run()
    //    {
    //        //Quick and easy.
    //        try
    //        {
    //            wav.Play(); //async
    //            wav.Dispose();
    //        }
    //        catch (Exception e)
    //        {
    //            // TODO : Notify error.
    //        }
    //    }
    //}

#endregion
}
