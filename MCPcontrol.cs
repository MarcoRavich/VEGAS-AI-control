using System;
using System.Windows.Forms;
using ScriptPortal.Vegas;

public class EntryPoint {
    public void FromVegas(Vegas vegas) {
        // Example: Show menu for manual testing
        var menu = new ContextMenu();
        menu.MenuItems.Add("Play", (s, e) => Play(vegas));
        menu.MenuItems.Add("Stop", (s, e) => Stop(vegas));
        menu.MenuItems.Add("Add Track", (s, e) => AddTrack(vegas));
        menu.MenuItems.Add("Mute First Track", (s, e) => MuteTrack(vegas, 0));
        menu.MenuItems.Add("Set Master Volume -6dB", (s, e) => SetMasterVolume(vegas, -6.0));
        menu.Show(new Form(), System.Windows.Forms.Cursor.Position);
    }

    // 1. Transport Control
    public void Play(Vegas vegas) { vegas.Transport.Play(); }
    public void Stop(Vegas vegas) { vegas.Transport.Stop(); }
    public void Record(Vegas vegas) { vegas.Transport.Record(); }
    
    // 2. Track Management
    public void AddTrack(Vegas vegas) {
        Track newTrack = new AudioTrack();
        vegas.Project.Tracks.Add(newTrack);
    }
    public void DeleteTrack(Vegas vegas, int idx) {
        if (idx < vegas.Project.Tracks.Count)
            vegas.Project.Tracks.RemoveAt(idx);
    }

    // 3. Track Parameter Control
    public void MuteTrack(Vegas vegas, int idx) {
        if (idx < vegas.Project.Tracks.Count)
            vegas.Project.Tracks[idx].Mute = true;
    }
    public void SetTrackVolume(Vegas vegas, int idx, double db) {
        if (idx < vegas.Project.Tracks.Count)
            vegas.Project.Tracks[idx].Volume = AudioBus.DBToAmplitude(db);
    }
    
    // 4. FX Control (Device Parameter)
    public void SetTrackFXParameter(Vegas vegas, int trackIdx, int fxIdx, int paramIdx, double value) {
        var track = vegas.Project.Tracks[trackIdx] as AudioTrack;
        if (track == null) return;
        if (fxIdx < track.AudioFX.Count) {
            var fx = track.AudioFX[fxIdx];
            if (paramIdx < fx.ParameterCount)
                fx.Parameters[paramIdx].Value = value;
        }
    }

    // 5. Automation (Envelope) Example
    public void SetVolumeEnvelope(Vegas vegas, int trackIdx, double time, double db) {
        var track = vegas.Project.Tracks[trackIdx] as AudioTrack;
        if (track == null) return;
        var env = track.Envelopes.FindByType(EnvelopeType.Volume);
        if (env == null) env = track.Envelopes.Add(EnvelopeType.Volume);
        env.Points.Add(new EnvelopePoint(Timecode.FromSeconds(time), AudioBus.DBToAmplitude(db), CurveType.Linear));
    }
    
    // 6. Export/Render
    public void RenderProject(Vegas vegas, string outputPath) {
        var renderArgs = new RenderArgs();
        renderArgs.OutputFile = outputPath;
        renderArgs.RenderTemplate = vegas.Renderers[0].Templates[0]; // Use default
        vegas.Render(renderArgs);
    }

    // 7. Project Settings
    public void SetTempo(Vegas vegas, double bpm) {
        vegas.Project.Tempo = bpm;
    }
    public void SetMasterVolume(Vegas vegas, double db) {
        vegas.MasterBus.OutputGain = AudioBus.DBToAmplitude(db);
    }
}
