import * as React from 'react';
import {
  ConnectDragPreview,
  ConnectDragSource,
  ConnectDropTarget,
  DragSource,
  DragSourceConnector,
  DragSourceMonitor,
  DragSourceSpec,
  DropTarget,
  DropTargetConnector,
  DropTargetMonitor,
  DropTargetSpec,
} from 'react-dnd';
import * as ReactDOM from 'react-dom';

const dragSourceSpec: DragSourceSpec<IDroppableDraggableContainerProps, any> = {
  beginDrag(props: IDroppableDraggableContainerProps) {
    return {
      ...props,
    };
  },
  canDrag(props: IDroppableDraggableContainerProps) {
    if (!props.canDrag) {
      return false;
    }
    return true;
  },
  isDragging(props: IDroppableDraggableContainerProps, monitor: DragSourceMonitor) {
    return props.id === monitor.getItem().id;
  },
};

const dropTargetSpec: DropTargetSpec<IDroppableDraggableContainerProps> = {
  drop(props: IDroppableDraggableContainerProps, monitor: DropTargetMonitor, Component: React.Component) {
    if (monitor.isOver({ shallow: true })) {
      switch (monitor.getItemType()) {
        case 'TOOLBAR_ITEM': {
          const toolbarItem = monitor.getItem();
          if (!toolbarItem.onDrop) {
            console.warn('Draggable Item doesn\'t have an onDrop-event');
            break;
          }
          toolbarItem.onDrop(props.id);
          break;
        }
        case 'ITEM': {
          const draggedComponent = monitor.getItem();
          let hoverOverIndex = props.index;
          const hoverBoundingRect = (ReactDOM.findDOMNode(Component) as Element).getBoundingClientRect();
          const hoverMiddleY = (hoverBoundingRect.bottom - hoverBoundingRect.top) / 2;
          const clientOffset = monitor.getClientOffset();
          const hoverClientY = clientOffset.y - hoverBoundingRect.top;

          if (hoverClientY > hoverMiddleY) {
            hoverOverIndex += 1;
          }

          props.onDropComponent(
            draggedComponent.id,
            hoverOverIndex,
            props.id,
            draggedComponent.containerId,
          );

          draggedComponent.index = hoverOverIndex;
          break;
        }
        case 'CONTAINER': {
          const draggedContainer = monitor.getItem();

          if (props.baseContainer) {
            // We can't get the index here, so let's not do anything
            props.onDropContainer(
              draggedContainer.id,
              0,
              props.id,
              draggedContainer.parentContainerId,
            );
            break;
          } else {
            let hoverOverIndex: number;
            if (!props.getIndex) {
              hoverOverIndex = props.index;
            } else {
              hoverOverIndex = props.getIndex(props.id);
            }

            const hoverBoundingRect = (ReactDOM.findDOMNode(Component) as Element).getBoundingClientRect();
            const hoverMiddleY = (hoverBoundingRect.bottom - hoverBoundingRect.top) / 2;
            const clientOffset = monitor.getClientOffset();
            const hoverClientY = clientOffset.y - hoverBoundingRect.top;

            if (hoverClientY > hoverMiddleY && props.id !== 'placeholder') {
              hoverOverIndex += 1;
            }

            props.onDropContainer(
              draggedContainer.id,
              hoverOverIndex,
              props.id,
              draggedContainer.containerId,
            );
          }
          break;
        }
      }
    }
  },
};

export interface IDroppableDraggableContainerProps {
  baseContainer: boolean;
  canDrag: boolean;
  id: string;
  index?: number;
  parentContainerId?: string;
  getIndex?: (containerId: string, parentContainerId?: string) => number;
  onMoveComponent?: (...args: any) => void;
  onDropComponent?: (...args: any) => void;
  onMoveContainer?: (...args: any) => void;
  onDropContainer?: (...args: any) => void;
}

class DroppableDraggableContainer extends React.Component<IDroppableDraggableContainerProps &
{
  connectDragPreview: ConnectDragPreview;
  connectDragSource: ConnectDragSource;
  connectDropTarget: ConnectDropTarget;
  isOver: boolean;
},
  any
  > {
  public render() {
    const {
      connectDropTarget,
      connectDragPreview,
      connectDragSource,
      isOver,
    } = this.props;

    return connectDropTarget(connectDragPreview(connectDragSource(
      <div
        style={{
          border: '1px solid #ccc',
          padding: '1em',
          marginBottom: -1,
          backgroundColor: isOver ? 'lightgrey' : 'white',
        }}
      >
        {this.props.children}
      </div>,
    )));
  }
}

export default DropTarget(
  ['TOOLBAR_ITEM', 'CONTAINER', 'ITEM'],
  dropTargetSpec,
  (connect: DropTargetConnector, monitor: DropTargetMonitor) => ({
    connectDropTarget: connect.dropTarget(),
    isOver: monitor.isOver({ shallow: true }),
  }),
)(
  DragSource(
    'CONTAINER',
    dragSourceSpec,
    (connect: DragSourceConnector, monitor: DragSourceMonitor) => ({
      connectDragSource: connect.dragSource(),
      connectDragPreview: connect.dragPreview(),
      isDragging: monitor.isDragging(),
    }),
  )(
    DroppableDraggableContainer,
  ),
);
