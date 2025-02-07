namespace UnityGLTF.Interactivity.Frontend
{
    public class FlowSocketUI : SocketUI
    {
        public NodeFlow flow { get; private set; }

        public void SetFlow(NodeFlow flow)
        {
            this.flow = flow;
            _title.text = flow.id;
        }

        protected override void ConnectSockets(SocketUI input, SocketUI output)
        {
            var i = input as FlowSocketUI;
            var o = output as FlowSocketUI;

            output.nodeUI.node.AddFlow(o.flow.id, i.nodeUI.node, i.flow.id);
        }
    }
}