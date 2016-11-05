function Message(id, userName, value, dateCreated, productId, messageWasRead) {
    this.id = id;
    this.value = value;
    this.dateCreated = dateCreated;
    this.productId = productId;
    this.userName = userName;
    this.messageWasRead = messageWasRead;
}