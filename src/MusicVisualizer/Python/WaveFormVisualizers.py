
import math

class WaveFormVisualizer:
    
    LineSmooth = None
    LineRough = None
    
    DataScale = 150.0

    def __init__(self):
        self.LineSmooth = Shell.CreateLine(256)
        self.LineRough = Shell.CreateLine(256)

    def Update(self, elapsedTime, audioData):
        pointSamples = audioData.FFTLength()
        targetSamples = pointSamples

        if (targetSamples > AppShell.Width):
            targetSamples = AppShell.Width

        step = pointSamples / targetSamples
        centerHeight = AppShell.Height / 2.0
        sampleWidth = AppShell.Width / targetSamples

        self.LineSmooth.Resize(targetSamples)
        self.LineRough.Resize(targetSamples)

        for i in range(0, targetSamples):
            index = i * step
            currentX = sampleWidth * i
            self.LineSmooth.SetAtPosition(i, Vector2(currentX, centerHeight + audioData.GetSmoothFFT(index) * self.DataScale), AppShell.ColorPalette.Color3)
            self.LineRough.SetAtPosition(i, Vector2(currentX, centerHeight + audioData.GetRoughFFT(index) * self.DataScale), AppShell.ColorPalette.Color2)
          

class CircleWaveFormVisualizer:
    
    LineSmooth = None
    LineRough = None

    Radius = 150.0
    DataScale = 150.0

    def __init__(self):
        self.LineSmooth = Shell.CreateLine(256)
        self.LineRough = Shell.CreateLine(256)
        
    def Update(self, elapsedTime, audioData):
        fftLength = audioData.FFTLength()
        
        self.LineSmooth.Resize(fftLength)
        self.LineRough.Resize(fftLength)
        
        center = Vector2(Shell.Width / 2, Shell.Height / 2)
        max = 2.0 * math.pi;
        step = max / fftLength;

        i = 0
        for s in range(0, fftLength):
            theta = s * step

            currentPosition = Vector2(Radius * math.cos(theta), Radius * math.sin(theta))
            currentNormal = currentPosition - Vector2((Radius + 1) * math.cos(theta), (Radius + 1) * math.sin(theta))
            
            self.LineSmooth.SetAtPosition(i,  currentPosition + (currentNormal * self.DataScale * audioData.GetSmoothFFT(i)), Color4(255, 0, 0, 0))
            self.LineRough.SetAtPosition(i, currentPosition + (currentNormal * self.DataScale * audioData.GetRoughFFT(i)), Color4(255, 0, 0, 0))

            i = i + 1
