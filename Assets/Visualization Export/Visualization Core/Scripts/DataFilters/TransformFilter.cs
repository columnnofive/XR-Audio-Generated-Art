using UnityEngine;

public class TransformFilter : DataFilter
{
    public Transform dataTransform;

    public enum TransformDataComponent
    {
        Position,
        Rotation,
        Scale
    }

    public TransformDataComponent transformDataComponent = TransformDataComponent.Position;

    public enum TransformDataSource
    {
        X, Y, Z,
        XY, XZ, YZ,
        XYZ
    }

    public TransformDataSource transformDataSource;

    public float minTransformValue = -1f;
    public float maxTransformValue = 1f;

    public override VisualizationData filter(VisualizationData dataToFilter)
    {
        float dataScaleFactor = getScaleFactor(dataTransform, transformDataComponent, transformDataSource);

        VisualizationData filteredData = dataToFilter;

        filteredData.audioBands = scale(filteredData.audioBands, dataScaleFactor);
        filteredData.amplitude *= dataScaleFactor;

        return filteredData;
    }

    private float getScaleFactor(Transform data, TransformDataComponent dataComponent,
        TransformDataSource dataSource)
    {
        float componentData = 0f;

        switch (dataComponent)
        {
            case TransformDataComponent.Position:
                componentData = getTransformComponentData(data.position, dataSource);
                break;
            case TransformDataComponent.Rotation:
                float rawComponentData = getTransformComponentData(data.eulerAngles, dataSource);
                if (rawComponentData <= 180f)
                    componentData = rawComponentData;
                else
                    componentData = rawComponentData.remap(180f, 360f, -180f, 0f);
                break;
            case TransformDataComponent.Scale:
                componentData = getTransformComponentData(data.lossyScale, dataSource);
                break;
        }

        float clampedComponentData = Mathf.Clamp(componentData, minTransformValue, maxTransformValue);
        return (clampedComponentData - minTransformValue) / (maxTransformValue - minTransformValue);
    }

    private float getTransformComponentData(Vector3 componentData, TransformDataSource dataSource)
    {
        switch (dataSource)
        {
            case TransformDataSource.X:
                return componentData.x;
            case TransformDataSource.Y:
                return componentData.y;
            case TransformDataSource.Z:
                return componentData.z;
            case TransformDataSource.XY:
                return (componentData.x + componentData.y) / 2;
            case TransformDataSource.XZ:
                return (componentData.x + componentData.z) / 2;
            case TransformDataSource.YZ:
                return (componentData.y + componentData.z) / 2;
            case TransformDataSource.XYZ:
                return (componentData.x + componentData.y + componentData.z) / 2;
            default:
                return 0f;
        }
    }

    private float[] scale(float[] data, float scaleFactor)
    {
        for (int i = 0; i < data.Length; i++)
            data[i] *= scaleFactor;
        return data;
    }
}
