using System;
using System.Collections.Generic;

namespace ImageTagger
{
    public interface IUndoCommand
    {
        void Undo();
        string Description { get; }
    }

    public class UndoManager
    {
        private readonly LinkedList<IUndoCommand> _undoStack = new LinkedList<IUndoCommand>();
        private readonly int _capacity;

        public event EventHandler StateChanged;

        public UndoManager(int capacity = 10)
        {
            _capacity = capacity;
        }

        public void Push(IUndoCommand command)
        {
            _undoStack.AddLast(command);
            if (_undoStack.Count > _capacity)
            {
                _undoStack.RemoveFirst();
            }
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var cmd = _undoStack.Last.Value;
                _undoStack.RemoveLast();
                cmd.Undo();
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanUndo => _undoStack.Count > 0;
        
        public string GetLastActionDescription()
        {
            return _undoStack.Count > 0 ? _undoStack.Last.Value.Description : string.Empty;
        }
    }
}
