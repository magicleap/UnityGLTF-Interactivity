using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class FlowUI : MonoBehaviour
    {
        private const int BEZIER_POINTS = 2;

        [Header("Self References")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private Image[] _dots;

        private Transform _from;
        private Transform _to;

        private Shape _shape;
        private Scene _scene;
        private VectorUtils.TessellationOptions _tesselationOptions;

        private Mesh _mesh;

        public void SetData(Transform from, Transform to)
        {
            _from = from;
            _to = to;
            GenerateCurve();
        }

        private void GenerateCurve()
        {
            _shape = new Shape
            {
                PathProps = new PathProperties { Stroke = new Stroke { Color = Color.red, HalfThickness = 1.0f } },
                Contours = new[] { new BezierContour { Segments = new BezierPathSegment[BEZIER_POINTS], Closed = false } },
                Fill = new SolidFill()
            };
            _scene = new Scene { Root = new SceneNode { Shapes = new List<Shape> { _shape } } };
            _tesselationOptions = new VectorUtils.TessellationOptions { StepDistance = 500, MaxCordDeviation = 0.1f, MaxTanAngleDeviation = 0.1f, SamplingStepSize = 0.01f };

            // Generate Mesh
            _mesh = new Mesh();
            _mesh.name = "Bezier Curve for Flow";
            _mesh.MarkDynamic();
            _meshFilter.mesh = _mesh;

            Generate();
        }

        public void Generate()
        {
            var localFrom = transform.InverseTransformPoint(_from.position);
            var localTo = transform.InverseTransformPoint(_to.position);

            var offsetFrom = localFrom + new Vector3(100, 0, 0);
            var offsetTo = localTo - new Vector3(100, 0, 0);

            _shape.Contours[0].Segments[0] = new BezierPathSegment()
            {
                P0 = localFrom,
                P1 = offsetFrom,
                P2 = offsetTo,
            };

            _shape.Contours[0].Segments[1] = new BezierPathSegment()
            {
                P0 = localTo,
                P1 = offsetTo,
                P2 = offsetFrom,
            };

            _dots[0].transform.localPosition = localFrom;
            _dots[1].transform.localPosition = localTo;

            var geoms = VectorUtils.TessellateScene(_scene, _tesselationOptions);
            VectorUtils.FillMesh(_mesh, geoms, 1f);
        }

        private void Update()
        {
            // TODO: Make this event-based so we only update meshes when the position of to and from change.
            // Possibly using a notifying property on the metadata position value or something but that won't work for the temp line when creating a new connection.
            Generate();
        }

        private void OnDestroy()
        {
            Destroy(_mesh);
        }
    }
}