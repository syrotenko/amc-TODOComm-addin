using TODOComm.Models;

namespace TODOComm.Commands {
    abstract class CommandParent {
        abstract public Comment CommentObj { get; set; }
    }
}
