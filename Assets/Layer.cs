using UnityEngine;

[System.Serializable]
public class Layer
{
    public Layer PreviousLayer; // in a network the layer before it
    public float[,] Weights;
    public float[] Biases;
    public float[] Values;

    // Initializes everything to 0
    public Layer(int nodes)
    {
        Values = new float[nodes];
        Biases = new float[nodes];
        Weights = null;
    }

    // How Hidden Layers are made
    public Layer(Layer previous, int nodes)
    {
        PreviousLayer = previous;
        Weights = new float[nodes, previous.Values.Length];
        Biases = new float[nodes];
        Values = new float[nodes];
    }

    public Layer cloneLayer()
    {
        Layer newLayer = new Layer(Values.GetLength(0));
        if (Weights != null)
        {
            newLayer.Weights = Weights.Clone() as float[,];
        }
        if (Biases != null)
        {
            newLayer.Biases = Biases.Clone() as float[];
        }
        newLayer.Values = Values.Clone() as float[];
        return newLayer;
    }

    public void UpdateLayerValues(float[] vals)
    {
        for (int i = 0; i < Values.Length; i++)
        {
            Values[i] = vals[i];
        }
    }

    // This is the updating of this layer from last layer of this layer
    public void ForwardPass()
    {
        for (int i = 0; i < Values.Length; i++)
        {
            Values[i] = 0;

            for (int j = 0; j < PreviousLayer.Values.Length; j++)
            {
                Values[i] += PreviousLayer.Values[j] * Weights[i, j];
            }

            Values[i] += Biases[i];
            Values[i] = _Function.Sigmoid(Values[i]);
        }
    }

    public void AssignRandomWeights()
    {
        for (int i = 0; i < Values.Length; i++)
        {
            if (Weights != null)
            {
                for (int j = 0; j < PreviousLayer.Values.Length; j++)
                {
                    Weights[i, j] = Random.Range(-2f, 2f);
                }
            }
            Biases[i] = Random.Range(-5f, 5f);
        }
    }

    public void RandomMutation(float threshold, float mut_degree)
    {
        for (int i = 0; i < Values.Length; i++)
        {
            if (Weights != null)
            {
                for (int j = 0; j < PreviousLayer.Values.Length; j++)
                {
                    if (Random.Range(0f, 1f) < threshold)
                    {
                        Weights[i, j] += Random.Range(-1f, 1f) * mut_degree;
                    }
                }
            }
            if (Random.Range(0f, 1f) < threshold)
            {
                Biases[i] += Random.Range(-1f, 1f) * mut_degree;
            }
        }
    }

    public string toString()
    {
        string str = "Layer - ";

        if (Weights != null)
        {
            int rowLength = Weights.GetLength(0);
            int colLength = Weights.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    str += string.Format("{0} ", Weights[i, j]);
                }
                str += "\n";
            }
        }
        else
        {
            str += " NULL";
        }

        return str;
    }
}