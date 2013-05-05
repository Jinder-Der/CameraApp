//
// Created by Owen Evans on 27/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>

@class Photo;
@class PhotoCellView;

@protocol PhotoCellViewDelegate
  -(void)cellImageWasTapped:(PhotoCellView *)cell;
@end

@interface PhotoCellView : UITableViewCell<UIGestureRecognizerDelegate>

@property (weak,nonatomic) IBOutlet UIImageView *thumbnailImage;
@property (strong, readwrite) Photo *photo;
@property (weak, nonatomic) IBOutlet UILabel *photoLabel;
@property (strong, readwrite) id<PhotoCellViewDelegate> delegate;

@end