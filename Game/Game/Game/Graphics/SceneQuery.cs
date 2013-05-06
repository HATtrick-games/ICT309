using System.Collections.Generic;
using DigitalRune.Graphics;
using DigitalRune.Graphics.SceneGraph;


namespace ICT309Game.Graphics
{
    // A scene query (see documentation of ISceneQuery) which sorts the queried
    // nodes into lists as required by the DeferredLightingScreen.
    public class SceneQuery : ISceneQuery
    {
        public SceneNode ReferenceNode { get; private set; }

        // All scene nodes where IsRenderable is true (e.g. MeshNodes).
        public List<SceneNode> RenderableNodes { get; private set; }


        public SceneQuery()
        {
            RenderableNodes = new List<SceneNode>();
        }


        public void Reset()
        {
            ReferenceNode = null;

            RenderableNodes.Clear();
        }


        public void Set(SceneNode referenceNode, IList<SceneNode> nodes)
        {
            Reset();
            ReferenceNode = referenceNode;

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];

                if (node.IsRenderable)
                {
                    RenderableNodes.Add(node);
                }
            }
        }
    }
}
