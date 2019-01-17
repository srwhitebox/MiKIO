using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xSpeech {
    public class xcSpeech {

        private SpeechSynthesizer SpeechSynthesizer = new SpeechSynthesizer();

        public int Volume {
            get {
                return this.SpeechSynthesizer.Volume;
            }
            set {
                SpeechSynthesizer.Volume = value;
            }
        }

        public int Speed {
            get {
                return this.SpeechSynthesizer.Rate;
            }
            set {
                this.SpeechSynthesizer.Rate = value;
            }
        }

        public IReadOnlyCollection<InstalledVoice> Voices {
            get {
                return SpeechSynthesizer.GetInstalledVoices();
            }
        }

        public void speak(string text) {
            SpeechSynthesizer.SpeakAsync(text);
        }

        /// <summary>
        /// Set Language
        /// Language Name ko-KR, zh-TW, ....
        /// </summary>
        /// <param name="langName"></param>
        public void setLanguage(string langName) {
            foreach (InstalledVoice voice in Voices) {
                if (voice.VoiceInfo.Culture.Name.Equals(langName)) {
                    selectVoice(voice);
                    return;
                }
            }
        }

        public void selectVoice(InstalledVoice voice) {
            SpeechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
        }

        public void selectVoice(string voiceName) {
            SpeechSynthesizer.SelectVoice(voiceName);
        }
    }
}
