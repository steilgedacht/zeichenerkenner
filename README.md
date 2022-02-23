# Zeichenerkenner

This is a software in which you can draw numbers and a neuronal network will then predict what you have drawn.</br>
You can finde the [Zeichenerkenner.exe in this directory](https://github.com/steilgedacht/zeichenerkenner/blob/main/Zeichenerkenner/bin/Debug/Zeichenerkenner.exe)</br>
As a basis the MNIST-Dataset was used for training.

![image](https://user-images.githubusercontent.com/89748204/155366550-1f3c2ccf-66ff-42a4-a22c-d9db8adf281e.png)

If you draw something, then you will get a prediction of what the neuronal network sees. Each number has a progress bar and you can see the percentage of each of the numbers. The number with the highest percentage get's displayed below.
On the top you can also find a Clean button to clear the canvas.

There is also a mode for inspecting in what the perfect number would look like.
Here the output of the neuronal network becomes the input. For each number from 0 to 9 you can see what would be the ideal input.
