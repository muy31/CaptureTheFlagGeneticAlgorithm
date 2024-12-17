using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class Brain
{
    public int NumInputs;
    public int NumOutputs;
    public int NumHiddenLayers;
    public int NumHiddenNodes;
    public Layer[] Layers;

    public Brain(int numI, int numO, int numLayers, int nodesPerLayer)
    {
        NumInputs = numI;
        NumOutputs = numO;
        NumHiddenLayers = numLayers;
        NumHiddenNodes = nodesPerLayer;

        Layers = new Layer[numLayers + 2];
        Layers[0] = new Layer(numI); // Input layer

        for (int i = 1; i < numLayers + 1; i++)
        {
            Layers[i] = new Layer(Layers[i - 1], nodesPerLayer);
            Layers[i].AssignRandomWeights();
        }

        Layers[numLayers + 1] = new Layer(Layers[numLayers], numO); // Output layer
        Layers[numLayers + 1].AssignRandomWeights();
    }

    public Brain(Layer[] layers)
    {
        Layers = layers;
    }

    public string toString()
    {
        string something = "";
        something += NumInputs + ", ";
        something += NumOutputs + ", ";
        something += NumHiddenLayers + ", ";
        something += NumHiddenNodes + "\n";

        foreach (Layer layer in Layers)
        {
            something += layer.toString() + "\n";
        }

        return "Brain " + something;
    }

    public Brain cloneAndMutate()
    {
        float mut_degree = UnityEngine.Random.Range(0f, 2f);
        float weight_mut_threshold = UnityEngine.Random.Range(0f, 0.3f);

        Layer[] new_layers = new Layer[NumHiddenLayers + 2];
        int index = 0;
        foreach (Layer layer in Layers)
        {
            new_layers[index] = layer.cloneLayer();
            if (index > 0) 
            { 
                new_layers[index].PreviousLayer = new_layers[index - 1];
            }
            new_layers[index].RandomMutation(weight_mut_threshold, mut_degree);
            index++;
        }

        Brain newBrain = new Brain(new_layers);
        newBrain.NumInputs = NumInputs;
        newBrain.NumOutputs = NumOutputs;
        newBrain.NumHiddenLayers = NumHiddenLayers;
        newBrain.NumHiddenNodes = NumHiddenNodes;

        return newBrain;
    }

    // Performs a complete forward pass of input data to output
    public float[] PredictOutput(float[] input)
    {
        Layers[0].UpdateLayerValues(input);

        for (int i = 1; i < NumHiddenLayers + 2; i++)
        {
            Layers[i].ForwardPass();
        }

        return _Function.OutputFinalizationFunction(Layers[NumHiddenLayers + 1].Values);
    }
}

public static class _Function
{
    public static float Sigmoid(double x)
    {
        return (float) Math.Tanh(x);
    }

    public static float[] OutputFinalizationFunction(float[] values)
    {
        return values;
    }
}